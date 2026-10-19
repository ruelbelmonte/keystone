namespace Keystone.Api.Customers;

public sealed record Customer
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Email { get; init; }

    public required DateTime CreatedAt { get; init; }
}
