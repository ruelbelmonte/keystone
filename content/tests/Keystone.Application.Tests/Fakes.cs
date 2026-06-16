using Keystone.Application.Common.Interfaces;
using Keystone.Domain.Customers;

namespace Keystone.Application.Tests;

/// <summary>In-memory repository so handlers can be unit-tested without EF.</summary>
public sealed class FakeCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<CustomerId, Customer> _store = [];

    public Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        _store[customer.Id] = customer;
        return Task.CompletedTask;
    }

    public Task<Customer?> GetByIdAsync(CustomerId id, CancellationToken cancellationToken) =>
        Task.FromResult(_store.GetValueOrDefault(id));
}

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => Task.FromResult(1);
}
