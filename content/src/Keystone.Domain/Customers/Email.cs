using Keystone.Domain.Common;

namespace Keystone.Domain.Customers;

/// <summary>An email address that cannot be constructed in an invalid state.</summary>
public sealed class Email : ValueObject
{
    private Email(string value) => Value = value;

    public string Value { get; }

    /// <summary>The only way to create an Email from untrusted input: validated.</summary>
    public static Result<Email> Create(string? input)
    {
        if (string.IsNullOrWhiteSpace(input) || !input.Contains('@', StringComparison.Ordinal))
        {
            return Result.Failure<Email>(CustomerErrors.InvalidEmail);
        }

        return Result.Success(new Email(input.Trim().ToLowerInvariant()));
    }

    /// <summary>
    /// Rehydrate from already-validated storage. For the persistence layer only;
    /// it skips validation because the value was valid when it was stored.
    /// </summary>
    public static Email FromTrusted(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
