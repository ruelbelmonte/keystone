using Keystone.Domain.Customers;

namespace Keystone.Application.Common.Interfaces;

/// <summary>
/// Persistence boundary for the Customer aggregate. Declared here in Application
/// and implemented in Infrastructure, so the dependency points inward.
/// </summary>
public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(CustomerId id, CancellationToken cancellationToken);
}
