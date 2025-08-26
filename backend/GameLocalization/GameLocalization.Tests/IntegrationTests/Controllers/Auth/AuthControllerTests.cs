using GameLocalization.Tests.TestSupport.Base;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Tests.TestSupport.Auth;

namespace GameLocalization.Tests.IntegrationTests.Controllers.Auth
{
    [Category("Integration")]
    public class AuthControllerTests : ApiAuthFlowTestBase
    {
        [Test]
        public async Task Register_Should_Create_User_And_Allow_Login()
        {
            var email = TestUsers.NewEmail();

            await TestUsers.RegisterAsync(Client, email);
            await TestUsers.LoginAsync(Client, email);

            Assert.Pass();
        }

        [Test]
        public async Task Login_Should_Set_AccessToken_Cookie_And_Me_Should_Return_Profile()
        {
            var email = TestUsers.NewEmail();
            await TestUsers.RegisterAsync(Client, email);

            var login = await Client
                .PostAsJsonAsync("/api/v1/auth/login", new { Email = email, Password = TestUsers.ValidPassword });

            login.StatusCode.Should().Be(HttpStatusCode.OK);
            login.Headers.TryGetValues("Set-Cookie", out var setCookies).Should().BeTrue();
            setCookies!.Any(v => v.Contains("access_token")).Should().BeTrue();

            var me = await Client.GetAsync("/api/v1/auth/me");
            me.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await me.Content.ReadFromJsonAsync<MeResponse>();
            dto!.Email.Should().Be(email);
            dto.Role.Should().Be(AppRoles.Member);
        }

        [Test]
        public async Task Logout_Should_Clear_Cookie_And_Me_Should_Return_401()
        {

            var email = TestUsers.NewEmail();
            await TestUsers.RegisterAsync(Client, email);
            await TestUsers.LoginAsync(Client, email);

            var logout = await Client.PostAsync("/api/v1/auth/logout", null);

            logout.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var me = await Client.GetAsync("/api/v1/auth/me");
            me.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Login_WrongPassword_Should_Return_401()
        {
            var email = TestUsers.NewEmail();
            await TestUsers.RegisterAsync(Client, email);

            var resp = await Client.PostAsJsonAsync("/api/v1/auth/login", new { Email = email, Password = "wrong123" });
            resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Me_WithoutAuth_Should_Return_401()
        {
            var resp = await Client.GetAsync("/api/v1/auth/me");

            resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    public record MeResponse(Guid Id, string Email, string Role);
}
