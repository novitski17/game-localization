using Asp.Versioning;
using GameLocalization.Api.Extensions;
using GameLocalization.Api.Helpers.Auth;
using GameLocalization.Api.Models.Requests.Auth;
using GameLocalization.Core.DTO.Auth;
using GameLocalization.Core.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Api.Models.Responses.Auth;
using AutoMapper;

namespace GameLocalization.Api.Controllers.Auth
{
    /// <summary>
    /// Provides endpoints for user authentication and profile management.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IAccessTokenCookieService _cookie;
        private readonly IMapper _mapper;

        public AuthController(IAuthService auth,
            IAccessTokenCookieService cookie,
            IMapper mapper)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            _cookie = cookie ?? throw new ArgumentNullException(nameof(cookie));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// Registers a new user with the <c>Member</c> role.
        /// </summary>
        /// <param name="request">Registration request containing email and password.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await _auth.RegisterAsync(_mapper.Map<RegisterDto>(request), ct);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Authenticates a user and issues an access token (stored in an HttpOnly cookie).
        /// </summary>
        /// <param name="request">Login request with email and password.</param>
        /// <param name="ct">Cancellation token.</param>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await _auth.LoginAsync(_mapper.Map<LoginDto>(request), ct);

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

        /// <summary>
        /// Gets the profile of the currently authenticated user.
        /// </summary>
        /// <remarks>
        /// Reads claims from the JWT token contained in the authentication cookie.
        /// </remarks>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Logs out the current user by removing the access token cookie.
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult Logout()
        {
            _cookie.Delete(Response);
            return NoContent();
        }
    }
}
