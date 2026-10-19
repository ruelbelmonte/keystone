using Keystone.Contracts.Customers;

namespace Keystone.Api.Customers;

public static class CustomerMappings
{
    public static CustomerResponse ToCustomerResponse(this Customer customer) =>
        new(customer.Id, customer.Name, customer.Email, customer.CreatedAt);
}
