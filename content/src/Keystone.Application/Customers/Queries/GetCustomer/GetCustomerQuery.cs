using Keystone.Domain.Common;
using MediatR;

namespace Keystone.Application.Customers.Queries.GetCustomer;

public sealed record GetCustomerQuery(Guid Id) : IRequest<Result<CustomerResponse>>;

public sealed record CustomerResponse(Guid Id, string Name, string Email);
