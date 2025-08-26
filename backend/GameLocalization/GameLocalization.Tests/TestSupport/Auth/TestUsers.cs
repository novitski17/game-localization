using System.Net.Http.Json;
using FluentAssertions;

namespace GameLocalization.Tests.TestSupport.Auth
{
    public static class TestUsers
    {
        public const string ValidPassword = "S3cure1234";

        public static string NewEmail() => $"user_{Guid.NewGuid():N}@example.com";

        public static async Task RegisterAsync(HttpClient client, string email, string password = ValidPassword)
        {
            var resp = await client
                .PostAsJsonAsync("/api/v1/auth/register", new { Email = email, Password = password });
            resp.IsSuccessStatusCode.Should().BeTrue();
        }

        public static async Task LoginAsync(HttpClient client, string email, string password = ValidPassword)
        {
            var resp = await client
                .PostAsJsonAsync("/api/v1/auth/login", new { Email = email, Password = password });
            resp.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
