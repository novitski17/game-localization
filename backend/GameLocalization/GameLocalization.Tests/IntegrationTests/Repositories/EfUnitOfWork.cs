using FluentAssertions;
using GameLocalization.Core.Domain.Entities;
using GameLocalization.Core.Exceptions;
using GameLocalization.Core.Interfaces.Repositories;
using GameLocalization.Tests.TestSupport.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameLocalization.Tests.IntegrationTests.Repositories
{
    [Category("Integration")]
    public class UnitOfWorkTests : IntegrationTestBase
    {
        private IUnitOfWork Uow => Services.GetRequiredService<IUnitOfWork>();

        [Test]
        public async Task SaveChangesAsync_Commits()
        {
            DbContext().Languages.Add(new Language
            {
                Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true
            });

            await Uow.SaveChangesAsync(CancellationToken.None);

            (await DbContext().Languages.CountAsync()).Should().Be(1);
        }

        [Test]
        public async Task SaveChangesAsync_Wraps_Exceptions_As_DataAccessException()
        {
            DbContext().Languages.Add(new Language
            {
                Id = Guid.NewGuid(), Code = "en-us", Name = "English", IsEnabled = true 
            });

            await DbContext().SaveChangesAsync();

            DbContext().Languages.Add(new Language { Id = Guid.NewGuid(), Code = "en-us", Name = "Dup", IsEnabled = true });

            Func<Task> act = async ()
                => await Uow.SaveChangesAsync(CancellationToken.None);

            await act.Should().ThrowAsync<DataAccessException>();
        }
    }
}
