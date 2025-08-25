using FluentResults;
using GameLocalization.Core.Domain.Constants;
using GameLocalization.Core.Domain.Entities.Identity;
using GameLocalization.Core.DTO.Auth;
using GameLocalization.Core.Interfaces.Providers;
using GameLocalization.Core.Interfaces.Repositories.Identity;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Interfaces.Services.Auth;
using FluentValidation;
using GameLocalization.Core.Errors;
using GameLocalization.Core.Validation;

namespace GameLocalization.Core.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenProvider _tokenProvider;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<RegisterDto> _registerValidator;


        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenProvider tokenProvider,
            IValidator<LoginDto> loginValidator,
            IValidator<RegisterDto> registerValidator)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
            _registerValidator = registerValidator ?? throw new ArgumentNullException(nameof(registerValidator));
        }

        public async Task<Result> RegisterAsync(RegisterDto dto, CancellationToken ct)
        {
            var validation = await _registerValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
            {
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var email = dto.Email.Trim();

            if (await _userRepository.ExistsByEmailAsync(email, ct))
            {
                return Result.Fail(new ConflictError($"User with email '{email}' already exists."));
            }

            var role = await _roleRepository.GetByNameAsync(AppRoles.Member, ct);
            if (role is null)
            {
                return Result.Fail(new ConfigurationError(
                    $"Required role '{AppRoles.Member}' is missing."));
            }

            var user = new User
            {
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                RoleId = role.Id,
            };

            await _userRepository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result<AuthenticatedUserDto>> LoginAsync(LoginDto dto, CancellationToken ct)
        {
            var validation = await _loginValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
            {
                return Result.Fail(ValidationErrorsHelper.ToValidationAppError(validation));
            }

            var email = dto.Email.Trim();

            var user = await _userRepository.GetByEmailWithRoleAsync(email, ct);
            if (user is null)
            {
                return Result.Fail(new UnauthorizedError("Invalid email or password."));
            }

            if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Result.Fail(new UnauthorizedError("Invalid email or password."));
            }

            var roleName = user.Role.Name;
            var token = _tokenProvider.GenerateAccessToken(user, roleName);

            var me = new AuthenticatedUserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = roleName,
                AccessToken = token,
            };

            return Result.Ok(me);
        }
    }
}
