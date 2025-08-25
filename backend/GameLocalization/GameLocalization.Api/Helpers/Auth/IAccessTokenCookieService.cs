namespace GameLocalization.Api.Helpers.Auth
{
    public interface IAccessTokenCookieService
    {
        void Set(HttpResponse response, string token);
        void Delete(HttpResponse response);
    }
}
