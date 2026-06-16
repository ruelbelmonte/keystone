namespace Keystone.Domain.Common;

/// <summary>Base class for entities: identity-based equality on a typed id.</summary>
public abstract class Entity<TId>
    where TId : notnull
{
    protected Entity(TId id) => Id = id;

    // Parameterless constructor for the ORM's materialisation.
    protected Entity() => Id = default!;

    public TId Id { get; protected init; }

    public override bool Equals(object? obj) =>
        obj is Entity<TId> other
        && other.GetType() == GetType()
        && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id);
}
