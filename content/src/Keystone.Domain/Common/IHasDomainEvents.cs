namespace Keystone.Domain.Common;

/// <summary>
/// Non-generic view of an aggregate's domain events, so the persistence layer can
/// collect and dispatch them without knowing each aggregate's id type.
/// </summary>
public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
