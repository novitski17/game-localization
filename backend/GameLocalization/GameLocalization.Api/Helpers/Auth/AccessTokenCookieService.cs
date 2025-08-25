using GameLocalization.Api.Configurations;
using GameLocalization.Core.Providers;
using Microsoft.Extensions.Options;

namespace GameLocalization.Api.Helpers.Auth
{
    public class AccessTokenCookieService : IAccessTokenCookieService
    {
        private readonly CookieAuthOptions _cookie;
        private readonly JwtOptions _jwt;

        public AccessTokenCookieService(
            IOptions<CookieAuthOptions> cookie,
            IOptions<JwtOptions> jwt)
        {
            _cookie = cookie.Value;
            _jwt = jwt.Value;
        }

        public void Set(HttpResponse response, string token)
        {
            var sameSite = _cookie.SameSite?.ToLowerInvariant();
            var opts = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = sameSite switch
                {
                    "none" => SameSiteMode.None,
                    "strict" => SameSiteMode.Strict,
                    _ => SameSiteMode.Lax
                },
                Expires = DateTimeOffset.UtcNow.AddMinutes(_jwt.AccessTokenMinutes),
                IsEssential = true
            };

            response.Cookies.Append(_cookie.Name, token, opts);
        }

        public void Delete(HttpResponse response)
        {
            response.Cookies.Delete(_cookie.Name, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });
        }
    }
}
