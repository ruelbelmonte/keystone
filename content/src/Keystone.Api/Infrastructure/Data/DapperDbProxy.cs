using System.Data;
using Dapper;

namespace Keystone.Api.Infrastructure.Data;

public sealed class DapperDbProxy : IDbProxy
{
    public Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken) =>
        connection.QueryAsync<T>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

    public Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken) =>
        connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

    public Task<int> ExecuteAsync(IDbConnection connection, string sql, object? parameters, CancellationToken cancellationToken) =>
        connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
}
