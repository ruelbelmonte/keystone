using System.Data;

namespace Keystone.Api.Infrastructure.Data;

/// <summary>
/// The only way repositories run SQL. Dapper's extension methods cannot be substituted in tests;
/// this interface can.
/// </summary>
public interface IDbProxy
{
    Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken);

    Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken);

    Task<int> ExecuteAsync(IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken);
}
