using GameLocalization.Core.Domain.Constants;

namespace GameLocalization.Tests.TestSupport.Auth
{
    public static class HttpClientRoleExtensions
    {
        public static void AsAdmin(this HttpClient client) =>
            SetRole(client, AppRoles.Admin);
        public static void AsMember(this HttpClient client) =>
            SetRole(client, AppRoles.Member);

        public static void ClearAuth(this HttpClient client)
            => client.DefaultRequestHeaders.Remove("X-User-Role");

        private static void SetRole(HttpClient client, string role)
        {
            client.DefaultRequestHeaders.Remove("X-User-Role");
            client.DefaultRequestHeaders.Add("X-User-Role", role);
        }
    }
}
