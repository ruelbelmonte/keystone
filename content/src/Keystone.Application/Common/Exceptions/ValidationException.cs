using FluentValidation.Results;

namespace Keystone.Application.Common.Exceptions;

/// <summary>
/// Thrown when input validation fails. Input validation is an exception (the
/// request was malformed); business-rule outcomes use Result instead. The API
/// maps this to a 400 ProblemDetails. See docs/adr/0004.
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures occurred.") =>
        Errors = failures
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(f => f.ErrorMessage).ToArray());

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
