using Keystone.Domain.Common;

namespace Keystone.Domain.Customers;

public static class CustomerErrors
{
    public static readonly Error InvalidEmail =
        new("Customer.InvalidEmail", "The email address is not valid.");

    public static readonly Error NameRequired =
        new("Customer.NameRequired", "A customer name is required.");

    public static Error NotFound(CustomerId id) =>
        new("Customer.NotFound", $"No customer was found with id {id.Value}.");
}
