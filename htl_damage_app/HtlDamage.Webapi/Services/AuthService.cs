using HtlDamage.Application.Dto;
using HtlDamage.Application.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Novell.Directory.Ldap;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Hosting;

namespace HtlDamage.Webapi.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly bool _isDevelopment;

        public AuthService(IHostEnvironment _env, IConfiguration config)
        {
            _config = config;
            _isDevelopment = _env.IsDevelopment();
        }

        public (bool isSuccess, object? payload, string? errorMessage) Login(CredentialsDto credentials)
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
                if (currentUser is null)
                {
                    return (false, null, "Invalid username or password");
                }

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

                return (true, new
                {
                    Username = currentUser.Cn,
                    IsAdmin = role == AdUserRole.Management,
                    Token = tokenHandler.WriteToken(token)
                }, null);
            }
            catch (LdapException e)
            {
                return (false, null, e.Message);
            }
            catch (ApplicationException e)
            {
                return (false, null, e.Message);
            }
        }

        public (AdUser? user, string? errorMessage) GetUserdata(ClaimsPrincipal user)
        {
            var adUserJson = user.Claims.FirstOrDefault(c => c.Type == "AdUser")?.Value;
            if (adUserJson is null) return (null, "User data not found in JWT token");
            var adUser = AdUser.FromJson(adUserJson);
            return (adUser, null);
        }
    }
}
