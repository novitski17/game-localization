namespace GameLocalization.Core.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken ct);
    }
}
