using FluentValidation;
using Keystone.Api.Customers.Repositories;
using Keystone.Contracts.Customers;

namespace Keystone.Api.Customers.Validators;

public sealed class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
{
    public RegisterCustomerRequestValidator(ICustomerRepository repository)
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        // Stop at the first failure so the database is only queried for a well-formed email.
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(320)
            .EmailAddress()
            .MustAsync(async (email, cancellationToken) => !await repository.EmailExists(email, cancellationToken))
            .WithMessage("A customer with this email is already registered.");
    }
}
