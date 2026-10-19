using Keystone.Api.Infrastructure.Data;
using Npgsql;

namespace Keystone.Api.Customers.Repositories;

public sealed class CustomerRepository(IDbConnectionFactory connectionFactory, IDbProxy dbProxy) : ICustomerRepository
{
    private const string SelectById = """
        select id, name, email, created_at as CreatedAt
        from customers
        where id = @Id
        """;

    private const string SelectEmailExists = """
        select exists (select 1 from customers where lower(email) = lower(@Email))
        """;

    private const string Insert = """
        insert into customers (id, name, email, created_at)
        values (@Id, @Name, @Email, @CreatedAt)
        """;

    public async Task<Customer?> GetCustomerById(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await dbProxy.QuerySingleOrDefaultAsync<Customer>(connection, SelectById, new { Id = id }, cancellationToken);
    }

    public async Task<bool> EmailExists(string email, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        return await dbProxy.QuerySingleOrDefaultAsync<bool>(connection, SelectEmailExists, new { Email = email }, cancellationToken);
    }

    public async Task<bool> InsertCustomer(Customer customer, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        try
        {
            return await dbProxy.ExecuteAsync(connection, Insert, customer, cancellationToken) == 1;
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            // The validator's EmailExists check lost a race with a concurrent insert.
            return false;
        }
    }
}
