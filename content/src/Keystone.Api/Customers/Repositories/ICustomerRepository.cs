namespace Keystone.Api.Customers.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerById(Guid id, CancellationToken cancellationToken);

    Task<bool> EmailExists(string email, CancellationToken cancellationToken);

    /// <returns>False when the email is already registered.</returns>
    Task<bool> InsertCustomer(Customer customer, CancellationToken cancellationToken);
}
