using System.Data.Common;

namespace Keystone.Api.Infrastructure.Data;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken);
}
