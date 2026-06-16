using Keystone.Application.Customers.Commands.RegisterCustomer;
using Keystone.Domain.Customers;

namespace Keystone.Application.Tests;

public class RegisterCustomerCommandHandlerTests
{
    [Fact]
    public async Task Registers_a_valid_customer()
    {
        var repository = new FakeCustomerRepository();
        var handler = new RegisterCustomerCommandHandler(repository, new FakeUnitOfWork());

        var result = await handler.Handle(
            new RegisterCustomerCommand("Ada", "ada@example.com"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
    }

    [Fact]
    public async Task Fails_with_invalid_email_error()
    {
        var handler = new RegisterCustomerCommandHandler(new FakeCustomerRepository(), new FakeUnitOfWork());

        var result = await handler.Handle(
            new RegisterCustomerCommand("Ada", "not-an-email"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(CustomerErrors.InvalidEmail, result.Error);
    }
}
