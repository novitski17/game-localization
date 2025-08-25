namespace GameLocalization.Core.Domain.Entities.Identity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public Role Role { get; set; } = default!;
        public Guid RoleId { get; set; }
    }
}
