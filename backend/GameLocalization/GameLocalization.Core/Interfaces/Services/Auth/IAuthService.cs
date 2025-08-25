using FluentResults;
using GameLocalization.Core.DTO.Auth;

namespace GameLocalization.Core.Interfaces.Services.Auth
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterDto dto, CancellationToken ct);
        Task<Result<AuthenticatedUserDto>> LoginAsync(LoginDto dto, CancellationToken ct);
    }
}
