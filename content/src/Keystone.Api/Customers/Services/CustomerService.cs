using System.Security.Claims;
using FluentValidation;
using Keystone.Api.Common;
using Keystone.Api.Customers.Repositories;
using Keystone.Api.Customers.Services.Interface;
using Keystone.Contracts.Customers;

namespace Keystone.Api.Customers.Services;

public sealed class CustomerService(
    ICustomerRepository repository,
    IValidator<RegisterCustomerRequest> registerValidator,
    TimeProvider timeProvider,
    ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<ServiceResult<CustomerResponse>> GetCustomer(Guid id, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        var customer = await repository.GetCustomerById(id, cancellationToken);
        if (customer is null)
        {
            return new ServiceResult<CustomerResponse>(ApiErrors.Customers.NotFound(id));
        }

        // Every read of personal data is audited: who, which record. Never the data itself.
        logger.LogInformation(
            "Audit: customer {CustomerId} read by {Subject} (client {ClientId})",
            customer.Id,
            user.FindFirstValue("sub"),
            user.FindFirstValue("client_id"));

        return new ServiceResult<CustomerResponse>(customer.ToCustomerResponse());
    }

    public async Task<ServiceResult<CustomerResponse>> RegisterCustomer(
        RegisterCustomerRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var validationResult = await registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ServiceResult<CustomerResponse>(validationResult.ToApiError());
        }

        var customer = new Customer
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            Email = request.Email,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
        };

        if (!await repository.InsertCustomer(customer, cancellationToken))
        {
            return new ServiceResult<CustomerResponse>(ApiErrors.Customers.EmailTaken);
        }

        // Ids only: names and emails are personal data and stay out of logs.
        logger.LogInformation(
            "Audit: customer {CustomerId} registered by {Subject} (client {ClientId})",
            customer.Id,
            user.FindFirstValue("sub"),
            user.FindFirstValue("client_id"));

        return new ServiceResult<CustomerResponse>(customer.ToCustomerResponse());
    }
}
