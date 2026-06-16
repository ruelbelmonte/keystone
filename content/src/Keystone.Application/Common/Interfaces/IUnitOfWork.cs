namespace Keystone.Application.Common.Interfaces;

/// <summary>Commits a unit of work. Implemented by the EF Core DbContext.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
