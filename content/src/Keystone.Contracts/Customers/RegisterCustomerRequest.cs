using System.ComponentModel.DataAnnotations;

namespace Keystone.Contracts.Customers;

// Annotations document the API in OpenAPI only; RegisterCustomerRequestValidator enforces the rules.

/// <summary>A request to register a new customer.</summary>
public sealed record RegisterCustomerRequest
{
    /// <summary>The customer's full name.</summary>
    [StringLength(200, MinimumLength = 1)]
    public required string Name { get; init; }

    /// <summary>The customer's email address. Unique, case-insensitive.</summary>
    [EmailAddress]
    [StringLength(320)]
    public required string Email { get; init; }
}
