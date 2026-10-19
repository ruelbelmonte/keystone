using Keystone.Api.Customers.Repositories;
using Keystone.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Keystone.Api.Tests.Infrastructure;

/// <summary>Hosts the API in memory with every IO dependency replaced by a substitute.</summary>
public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public ICustomerRepository CustomerRepository { get; } = Substitute.For<ICustomerRepository>();

    /// <summary>Extra service overrides, applied when the host is built on the first CreateClient().</summary>
    public Action<IServiceCollection>? ConfigureTestServices { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton(Substitute.For<IDbConnectionFactory>());

            services.RemoveAll<ICustomerRepository>();
            services.AddSingleton(CustomerRepository);

            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            ConfigureTestServices?.Invoke(services);
        });
    }
}
