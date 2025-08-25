namespace GameLocalization.Api.Models.Requests.Auth
{
    public class RegisterRequest
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
    }
}
