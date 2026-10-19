namespace Keystone.Contracts.Customers;

/// <summary>A registered customer.</summary>
/// <param name="Id">The customer id.</param>
/// <param name="Name">The customer's full name.</param>
/// <param name="Email">The customer's email address.</param>
/// <param name="CreatedAt">When the customer was registered (UTC).</param>
public sealed record CustomerResponse(Guid Id, string Name, string Email, DateTime CreatedAt);
