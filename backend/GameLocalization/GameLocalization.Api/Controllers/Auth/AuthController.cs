using Asp.Versioning;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Helpers.Auth;
using GameLocalization.Core.DTO.Auth;
using GameLocalization.Core.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Api.Models.Responses.Auth;

namespace GameLocalization.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IAccessTokenCookieService _cookie;

        public AuthController(IAuthService auth, IAccessTokenCookieService cookie)
        {
            _auth = auth;
            _cookie = cookie;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
        {
            var result = await _auth.RegisterAsync(dto, ct);
            return this.ToActionResult(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            var result = await _auth.LoginAsync(dto, ct);

            return this.ToActionResult(result, me =>
            {
                _cookie.Set(Response, me.AccessToken);
                return Ok(new LoginResponse
                {
                    Id = me.Id,
                    Email = me.Email,
                    Role = me.Role
                });
            });
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<MeResponse> GetMe()
        {
            var id = User.GetUserId();
            if (id is null) 
            {
                return Unauthorized();
            }

            var email = User.GetEmail();
            var role = User.GetRole();

            if (string.IsNullOrEmpty(role))
            {
                role = AppRoles.Member;
            }

            return Ok(new MeResponse
            {
                Id = id.Value,
                Email = email,
                Role = role
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            _cookie.Delete(Response);
            return NoContent();
        }
    }
}
