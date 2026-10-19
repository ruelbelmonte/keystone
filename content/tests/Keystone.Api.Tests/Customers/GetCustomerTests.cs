using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Keystone.Api.Common;
using Keystone.Api.Customers;
using Keystone.Api.Tests.Infrastructure;
using Keystone.Contracts;
using Keystone.Contracts.Customers;

namespace Keystone.Api.Tests.Customers;

public class GetCustomerTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Theory]
    [AutoData]
    public async Task GetCustomer_returns_200_with_customer_when_it_exists(Customer customer)
    {
        factory.CustomerRepository.GetCustomerById(customer.Id, Arg.Any<CancellationToken>()).Returns(customer);

        var response = await InvokeGetCustomer(customer.Id, scopes: Roles.ReadScope);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CustomerResponse>(JsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        body.Should().BeEquivalentTo(customer.ToCustomerResponse());
    }

    [Theory]
    [AutoData]
    public async Task GetCustomer_returns_404_when_customer_does_not_exist(Guid id)
    {
        factory.CustomerRepository.GetCustomerById(id, Arg.Any<CancellationToken>()).Returns((Customer?)null);

        var response = await InvokeGetCustomer(id);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var error = await response.Content.ReadFromJsonAsync<ApiError>(JsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        error!.Code.Should().Be(ApiErrors.Customers.NotFound(id).Code);
    }

    [Theory]
    [AutoData]
    public async Task GetCustomer_returns_401_when_caller_is_anonymous(Guid id)
    {
        var response = await InvokeGetCustomer(id, anonymous: true);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<HttpResponseMessage> InvokeGetCustomer(Guid id, string? scopes = null, bool anonymous = false)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"/v1/customers/{id}");
        if (scopes is not null)
        {
            message.Headers.Add(TestAuthHandler.ScopesHeader, scopes);
        }

        if (anonymous)
        {
            message.Headers.Add(TestAuthHandler.AnonymousHeader, "true");
        }

        return await factory.CreateClient().SendAsync(message, TestContext.Current.CancellationToken);
    }
}
