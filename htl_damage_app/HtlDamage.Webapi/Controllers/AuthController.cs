using HtlDamage.Application.Dto;
using HtlDamage.Application.Infrastructure;
using HtlDamage.Webapi.Services;
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
    public partial class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// POST /api/user/login
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] CredentialsDto credentials)
        {
            var (isSuccess, payload, errorMessage) = _authService.Login(credentials);
            if (!isSuccess) return Unauthorized(errorMessage);
            return Ok(payload);
        }

        /// <summary>
        /// GET /api/user/me
        /// Gets information about the current (authenticated) user.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetUserdata()
        {
            var (user, errorMessage) = _authService.GetUserdata(HttpContext.User);
            if (user is null) return BadRequest(errorMessage);
            return Ok(user);
        }
    }
}