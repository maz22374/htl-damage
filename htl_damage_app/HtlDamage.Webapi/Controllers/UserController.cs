using HtlDamage.Application.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Novell.Directory.Ldap;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace HtlDamage.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        // DTO class for the JSON body of the login request
        public record CredentialsDto(string Username, string Password);

        private readonly IConfiguration _config;
        private readonly bool _isDevelopment;

        public UserController(IHostEnvironment _env, IConfiguration config)
        {
            _config = config;
            _isDevelopment = _env.IsDevelopment();
        }

        /// <summary>
        /// POST /api/user/login
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] CredentialsDto credentials)
        {
            var lifetime = TimeSpan.FromHours(3);
            var searchuser = _config["Searchuser"];
            var searchpass = _config["Searchpass"];
            var secret = Convert.FromBase64String(_config["JwtSecret"]);
            var localAdmins = _config["LocalAdmins"].Split(",");
            try
            {
                using var service = _isDevelopment && !string.IsNullOrEmpty(searchuser)
                    ? AdService.Login(searchuser, searchpass, credentials.Username)
                    : AdService.Login(credentials.Username, credentials.Password);
                var currentUser = service.CurrentUser;
                if (currentUser is null) { return Unauthorized(); }
                var role = localAdmins.Contains(currentUser.Cn) ? AdUserRole.Management : currentUser.Role;

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // Payload for our JWT.
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, currentUser.Cn),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString()),
                        new Claim("AdUser", currentUser.ToJson())
                    }),
                    Expires = DateTime.UtcNow + lifetime,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(secret),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new
                {
                    Username = currentUser.Cn,
                    IsAdmin = role == AdUserRole.Management,
                    Token = tokenHandler.WriteToken(token)
                });
            }
            catch (LdapException e)
            {
                return Unauthorized(e.Message);
            }
            catch (ApplicationException e)
            {
                return Unauthorized(e.Message);
            }
        }

        /// <summary>
        /// GET /api/user/me
        /// Gets information about the current (authenticated) user.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetUserdata()
        {
            var adUserJson = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AdUser")?.Value;
            if (adUserJson is null) { return BadRequest(); }

            return Ok(AdUser.FromJson(adUserJson));
        }
    }
}