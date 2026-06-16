using Keystone.Domain.Common;

namespace Keystone.Domain.Customers;

/// <summary>
/// The Customer aggregate root. It can only be created through <see cref="Register"/>,
/// which guarantees a valid email and records that the registration happened.
/// </summary>
public sealed class Customer : AggregateRoot<CustomerId>
{
    private Customer(CustomerId id, string name, Email email)
        : base(id)
    {
        Name = name;
        Email = email;
    }

    // For the ORM. Properties are set during materialisation.
    private Customer()
    {
        Name = null!;
        Email = null!;
    }

    public string Name { get; private set; }

    public Email Email { get; private set; }

    public static Customer Register(string name, Email email)
    {
        var customer = new Customer(CustomerId.New(), name, email);
        customer.RaiseDomainEvent(new CustomerRegisteredDomainEvent(customer.Id));
        return customer;
    }
}
