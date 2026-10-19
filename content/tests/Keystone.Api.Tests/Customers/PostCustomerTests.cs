using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Keystone.Api.Common;
using Keystone.Api.Customers;
using Keystone.Api.Tests.Infrastructure;
using Keystone.Contracts;
using Keystone.Contracts.Customers;

namespace Keystone.Api.Tests.Customers;

public class PostCustomerTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Theory]
    [InlineAutoData("ada@example.com")]
    public async Task RegisterCustomer_returns_201_with_location_when_request_is_valid(string email, string name)
    {
        factory.CustomerRepository.EmailExists(email, Arg.Any<CancellationToken>()).Returns(false);
        factory.CustomerRepository.InsertCustomer(Arg.Any<Customer>(), Arg.Any<CancellationToken>()).Returns(true);

        var response = await InvokePostCustomer(new RegisterCustomerRequest { Name = name, Email = email });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var customer = await response.Content.ReadFromJsonAsync<CustomerResponse>(JsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        customer!.Name.Should().Be(name);
        customer.Email.Should().Be(email);
        response.Headers.Location!.ToString().Should().EndWith($"/v1/customers/{customer.Id}");
        await factory.CustomerRepository.Received(1).InsertCustomer(
            Arg.Is<Customer>(c => c.Id == customer.Id && c.Email == email),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineAutoData("not-an-email")]
    [InlineAutoData("")]
    public async Task RegisterCustomer_returns_400_when_email_is_invalid(string email, string name)
    {
        var response = await InvokePostCustomer(new RegisterCustomerRequest { Name = name, Email = email });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ApiError>(JsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        error!.Code.Should().Be(ApiErrors.ValidationFailedCode);
        error.Errors.Should().ContainKey(nameof(RegisterCustomerRequest.Email));
    }

    [Theory]
    [InlineAutoData("taken@example.com")]
    public async Task RegisterCustomer_returns_400_when_email_is_already_registered(string email, string name)
    {
        factory.CustomerRepository.EmailExists(email, Arg.Any<CancellationToken>()).Returns(true);

        var response = await InvokePostCustomer(new RegisterCustomerRequest { Name = name, Email = email });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ApiError>(JsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        error!.Errors.Should().ContainKey(nameof(RegisterCustomerRequest.Email));
    }

    [Theory]
    [InlineAutoData("raced@example.com")]
    public async Task RegisterCustomer_returns_409_when_email_is_registered_concurrently(string email, string name)
    {
        factory.CustomerRepository.EmailExists(email, Arg.Any<CancellationToken>()).Returns(false);
        factory.CustomerRepository.InsertCustomer(Arg.Is<Customer>(c => c.Email == email), Arg.Any<CancellationToken>()).Returns(false);

        var response = await InvokePostCustomer(new RegisterCustomerRequest { Name = name, Email = email });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var error = await response.Content.ReadFromJsonAsync<ApiError>(JsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        error!.Code.Should().Be(ApiErrors.Customers.EmailTaken.Code);
    }

    [Fact]
    public async Task RegisterCustomer_returns_400_problem_when_body_is_malformed()
    {
        var response = await InvokePostCustomer(new StringContent("{ not json", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
    }

    [Theory]
    [InlineAutoData("ada@example.com")]
    public async Task RegisterCustomer_returns_403_when_caller_lacks_write_scope(string email, string name)
    {
        var response = await InvokePostCustomer(new RegisterCustomerRequest { Name = name, Email = email }, scopes: Roles.ReadScope);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineAutoData("ada@example.com")]
    public async Task RegisterCustomer_returns_401_when_caller_is_anonymous(string email, string name)
    {
        var response = await InvokePostCustomer(new RegisterCustomerRequest { Name = name, Email = email }, anonymous: true);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private Task<HttpResponseMessage> InvokePostCustomer(RegisterCustomerRequest request, string? scopes = null, bool anonymous = false) =>
        InvokePostCustomer(JsonContent.Create(request, options: JsonSerializerOptions.Web), scopes, anonymous);

    private async Task<HttpResponseMessage> InvokePostCustomer(HttpContent content, string? scopes = null, bool anonymous = false)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "/v1/customers") { Content = content };
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
