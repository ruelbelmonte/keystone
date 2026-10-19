using System.Net;
using Keystone.Contracts;

namespace Keystone.Api.Common;

/// <summary>Every error the API can return, so codes and statuses stay consistent.</summary>
public static class ApiErrors
{
    public const string ValidationFailedCode = "validation_failed";

    public static class Customers
    {
        public static ApiError NotFound(Guid id) =>
            new("customer_not_found", $"Customer '{id}' was not found.", HttpStatusCode.NotFound);

        public static readonly ApiError EmailTaken =
            new("customer_email_taken", "A customer with this email is already registered.", HttpStatusCode.Conflict);
    }
}
