using FluentValidation;
using MediatR;
using ValidationException = Keystone.Application.Common.Exceptions.ValidationException;

namespace Keystone.Application.Common.Behaviours;

/// <summary>
/// Runs all registered validators for a request before it reaches its handler.
/// A request that fails validation never touches domain logic.
/// </summary>
public sealed class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(result => result.Errors).Where(f => f is not null).ToList();
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
