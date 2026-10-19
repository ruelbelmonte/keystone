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
    public async Task<ServiceResult<CustomerResponse>> GetCustomer(Guid id, CancellationToken cancellationToken)
    {
        var customer = await repository.GetCustomerById(id, cancellationToken);

        return customer is null
            ? new ServiceResult<CustomerResponse>(ApiErrors.Customers.NotFound(id))
            : new ServiceResult<CustomerResponse>(customer.ToCustomerResponse());
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
            "Customer {CustomerId} registered by {Subject}",
            customer.Id,
            user.FindFirstValue("sub"));

        return new ServiceResult<CustomerResponse>(customer.ToCustomerResponse());
    }
}
