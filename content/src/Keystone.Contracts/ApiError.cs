using System.Net;
using System.Text.Json.Serialization;

namespace Keystone.Contracts;

/// <summary>The body of every non-success response produced by a service.</summary>
/// <param name="Code">A stable, machine-readable error code.</param>
/// <param name="Message">A human-readable description.</param>
/// <param name="HttpStatusCode">The HTTP status the API responds with.</param>
public sealed record ApiError(string Code, string Message, HttpStatusCode HttpStatusCode)
{
    /// <summary>Field-level validation failures, keyed by property name.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; init; }
}
