using Npgsql;

namespace Keystone.Api.Infrastructure.Data;

public static class DataServiceCollectionExtensions
{
    public const string ConnectionStringName = "Keystone";

    public static IServiceCollection AddPostgres(this IServiceCollection services)
    {
        // Resolved lazily so hosts that substitute the repositories (tests) never need a connection string.
        services.AddSingleton(provider => NpgsqlDataSource.Create(
            provider.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException($"ConnectionStrings:{ConnectionStringName} is not configured.")));
        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
        services.AddSingleton<IDbProxy, DapperDbProxy>();

        services.AddHealthChecks().AddNpgSql();

        return services;
    }
}
