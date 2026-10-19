using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Keystone.Contracts;

namespace Keystone.Api.Common;

public static class ValidationExtensions
{
    public static ApiError ToApiError(this ValidationResult result) =>
        new(ApiErrors.ValidationFailedCode, "One or more validation errors occurred.", HttpStatusCode.BadRequest)
        {
            Errors = result.ToDictionary(),
        };

    /// <summary>
    /// Reads data the service fetched and passed in through <see cref="ValidationContext{T}.RootContextData"/>.
    /// Returns default when the key is absent, so rules can skip rather than fail.
    /// </summary>
    public static T? Extract<TRequest, T>(this ValidationContext<TRequest> context, string key) =>
        context.RootContextData.TryGetValue(key, out var value) && value is T typed ? typed : default;
}
