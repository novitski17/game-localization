namespace GameLocalization.Api.Configurations
{
    public class CookieAuthOptions
    {
        public string Name { get; init; } = "access_token";
        public string SameSite { get; init; } = "Lax";
    }
}
