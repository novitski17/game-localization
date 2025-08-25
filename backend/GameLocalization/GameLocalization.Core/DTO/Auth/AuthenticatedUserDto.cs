namespace GameLocalization.Core.DTO.Auth
{
    public class AuthenticatedUserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public string AccessToken { get; init; } = string.Empty;
    }
}
