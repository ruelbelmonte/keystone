namespace Keystone.Domain.Customers;

/// <summary>A strongly-typed id, so a CustomerId can never be passed where some
/// other Guid was meant.</summary>
public readonly record struct CustomerId(Guid Value)
{
    public static CustomerId New() => new(Guid.NewGuid());
}
