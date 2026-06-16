namespace Keystone.Domain.Common;

/// <summary>A failure with a stable code and a human-readable message.</summary>
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}
