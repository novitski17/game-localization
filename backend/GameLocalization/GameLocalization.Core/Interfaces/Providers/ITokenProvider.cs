using GameLocalization.Core.Domain.Entities.Identity;

namespace GameLocalization.Core.Interfaces.Providers
{
    public interface ITokenProvider
    {
        string GenerateAccessToken(User user, string role);
    }
}
