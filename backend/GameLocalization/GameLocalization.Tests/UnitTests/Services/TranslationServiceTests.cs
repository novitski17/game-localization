using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.DTO.Translations;
using GameLocalization.Core.Errors;
using GameLocalization.Core.Exceptions;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Core.Services;
using GameLocalization.Core.Validation.Translations;
using GameLocalization.Tests.TestSupport.Base;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace GameLocalization.Tests.UnitTests.Services
{
    [Category("Unit")]
    public class TranslationServiceTests : UnitTestBase
    {
        private Mock<ITranslationRepository> _translationRepositoryMock = null!;
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<ILogger<TranslationService>> _loggerMock = null!;
        private TranslationService _translationService = null!;

        [SetUp]
        public void SetUp()
        {
            _translationRepositoryMock = new Mock<ITranslationRepository>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            _loggerMock = new Mock<ILogger<TranslationService>>(MockBehavior.Loose);

            var updateValidator = new UpdateTranslationDtoValidator();

            _translationService = new TranslationService(
                _translationRepositoryMock.Object,
                _unitOfWorkMock.Object,
                updateValidator,
                _loggerMock.Object);
        }

        [Test]
        public async Task UpdateAsync_Should_Fail_When_Invalid_Model()
        {
            var dto = new UpdateTranslationDto { Value = null! };

            var result = await _translationService.UpdateAsync(Guid.NewGuid(), dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<ValidationAppError>().Should().HaveCount(1);

            _translationRepositoryMock.Verify(r => r.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_Should_Return_NotFound_When_Translation_Missing()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateTranslationDto { Value = "Play" };

            _translationRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Translation?)null);

            var result = await _translationService.UpdateAsync(id, dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<AppError>().Should().ContainSingle(e => e.HttpStatus == 404);

            _translationRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_Should_Update_Value_And_Save()
        {
            var id = Guid.NewGuid();
            var entity = new Translation { Id = id, Value = "Old" };
            var dto = new UpdateTranslationDto { Value = "New Value" };

            _translationRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _translationService.UpdateAsync(id, dto, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            entity.Value.Should().Be("New Value");

            _translationRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_Should_Return_DatabaseError_On_Save_Exception()
        {
            var id = Guid.NewGuid();
            var entity = new Translation { Id = id, Value = "Old" };
            var dto = new UpdateTranslationDto { Value = "New" };

            _translationRepositoryMock
                .Setup(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DataAccessException("boom"));

            var result = await _translationService.UpdateAsync(id, dto, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.OfType<DatabaseError>().Should().HaveCount(1);

            _translationRepositoryMock.Verify(r => r.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
