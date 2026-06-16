using Keystone.Application.Common.Interfaces;
using Keystone.Domain.Common;
using Keystone.Domain.Customers;
using MediatR;

namespace Keystone.Application.Customers.Queries.GetCustomer;

public sealed class GetCustomerQueryHandler(ICustomerRepository customers)
    : IRequestHandler<GetCustomerQuery, Result<CustomerResponse>>
{
    public async Task<Result<CustomerResponse>> Handle(
        GetCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var id = new CustomerId(request.Id);
        var customer = await customers.GetByIdAsync(id, cancellationToken);

        return customer is null
            ? Result.Failure<CustomerResponse>(CustomerErrors.NotFound(id))
            : Result.Success(new CustomerResponse(customer.Id.Value, customer.Name, customer.Email.Value));
    }
}
