using Keystone.Application.Common.Interfaces;
using Keystone.Domain.Customers;

namespace Keystone.Infrastructure.Persistence;

internal sealed class CustomerRepository(ApplicationDbContext context) : ICustomerRepository
{
    public async Task AddAsync(Customer customer, CancellationToken cancellationToken) =>
        await context.Customers.AddAsync(customer, cancellationToken);

    public async Task<Customer?> GetByIdAsync(CustomerId id, CancellationToken cancellationToken) =>
        await context.Customers.FindAsync([id], cancellationToken);
}
