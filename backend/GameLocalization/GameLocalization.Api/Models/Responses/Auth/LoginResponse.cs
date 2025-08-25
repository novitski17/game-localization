namespace GameLocalization.Api.Models.Responses.Auth
{
    public class LoginResponse
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
    }
}
