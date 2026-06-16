using Keystone.Application.Common.Interfaces;
using Keystone.Domain.Common;
using Keystone.Domain.Customers;
using MediatR;

namespace Keystone.Application.Customers.Commands.RegisterCustomer;

public sealed class RegisterCustomerCommandHandler(
    ICustomerRepository customers,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        RegisterCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        if (email.IsFailure)
        {
            return Result.Failure<Guid>(email.Error);
        }

        var customer = Customer.Register(request.Name, email.Value);
        await customers.AddAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(customer.Id.Value);
    }
}
