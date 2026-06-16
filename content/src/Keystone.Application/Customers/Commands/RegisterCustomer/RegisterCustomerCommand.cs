using Keystone.Domain.Common;
using MediatR;

namespace Keystone.Application.Customers.Commands.RegisterCustomer;

public sealed record RegisterCustomerCommand(string Name, string Email) : IRequest<Result<Guid>>;
