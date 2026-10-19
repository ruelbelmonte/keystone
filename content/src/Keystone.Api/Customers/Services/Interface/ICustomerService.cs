using System.Security.Claims;
using Keystone.Api.Common;
using Keystone.Contracts.Customers;

namespace Keystone.Api.Customers.Services.Interface;

public interface ICustomerService
{
    Task<ServiceResult<CustomerResponse>> GetCustomer(Guid id, ClaimsPrincipal user, CancellationToken cancellationToken);

    Task<ServiceResult<CustomerResponse>> RegisterCustomer(
        RegisterCustomerRequest request,
        ClaimsPrincipal user,
        CancellationToken cancellationToken);
}
