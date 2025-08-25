using System.Security.Claims;

namespace GameLocalization.Api.Helpers.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(sub, out var id) ? id : null;
        }

        public static string GetEmail(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        public static string GetRole(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
