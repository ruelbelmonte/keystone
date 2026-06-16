namespace Keystone.Domain.Common;

/// <summary>
/// A fact that has happened in the domain. Deliberately free of any messaging
/// framework: the Domain layer depends on nothing. Infrastructure wraps these
/// into notifications when it dispatches them. See docs/adr/0003.
/// </summary>
public interface IDomainEvent;
