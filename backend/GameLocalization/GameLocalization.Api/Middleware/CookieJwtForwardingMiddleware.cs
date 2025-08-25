namespace GameLocalization.Api.Middleware
{
    public class CookieJwtForwardingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _cookieName;

        public CookieJwtForwardingMiddleware(RequestDelegate next, IConfiguration cfg)
        {
            _next = next;
            _cookieName = cfg.GetSection("Auth:Cookie:Name").Value ?? "access_token";
        }

        public async Task Invoke(HttpContext ctx)
        {
            if (!ctx.Request.Headers.ContainsKey("Authorization") &&
                ctx.Request.Cookies.TryGetValue(_cookieName, out var token) &&
                !string.IsNullOrWhiteSpace(token))
            {
                ctx.Request.Headers.Append("Authorization", $"Bearer {token}");
            }

            await _next(ctx);
        }
    }

    public static class CookieJwtForwardingExtensions
    {
        public static IApplicationBuilder UseCookieJwtForwarding(this IApplicationBuilder app)
            => app.UseMiddleware<CookieJwtForwardingMiddleware>();
    }
}
