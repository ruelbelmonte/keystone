using Keystone.Domain.Customers;

namespace Keystone.Domain.Tests;

public class CustomerTests
{
    [Fact]
    public void Register_creates_a_customer_and_raises_a_domain_event()
    {
        var email = Email.Create("ada@example.com").Value;

        var customer = Customer.Register("Ada", email);

        Assert.Equal("Ada", customer.Name);
        Assert.Equal(email, customer.Email);
        var domainEvent = Assert.Single(customer.DomainEvents);
        var registered = Assert.IsType<CustomerRegisteredDomainEvent>(domainEvent);
        Assert.Equal(customer.Id, registered.CustomerId);
    }

    [Fact]
    public void ClearDomainEvents_empties_the_collection()
    {
        var customer = Customer.Register("Ada", Email.Create("ada@example.com").Value);

        customer.ClearDomainEvents();

        Assert.Empty(customer.DomainEvents);
    }
}
