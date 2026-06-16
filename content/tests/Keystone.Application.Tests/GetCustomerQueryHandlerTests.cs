using Keystone.Application.Customers.Queries.GetCustomer;
using Keystone.Domain.Customers;

namespace Keystone.Application.Tests;

public class GetCustomerQueryHandlerTests
{
    [Fact]
    public async Task Returns_the_customer_when_it_exists()
    {
        var repository = new FakeCustomerRepository();
        var customer = Customer.Register("Ada", Email.Create("ada@example.com").Value);
        await repository.AddAsync(customer, CancellationToken.None);
        var handler = new GetCustomerQueryHandler(repository);

        var result = await handler.Handle(new GetCustomerQuery(customer.Id.Value), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Ada", result.Value.Name);
        Assert.Equal("ada@example.com", result.Value.Email);
    }

    [Fact]
    public async Task Returns_a_not_found_failure_when_absent()
    {
        var handler = new GetCustomerQueryHandler(new FakeCustomerRepository());

        var result = await handler.Handle(new GetCustomerQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Customer.NotFound", result.Error.Code);
    }
}
