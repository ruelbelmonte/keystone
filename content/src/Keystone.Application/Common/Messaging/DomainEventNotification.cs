using Keystone.Domain.Common;
using MediatR;

namespace Keystone.Application.Common.Messaging;

/// <summary>
/// Wraps a pure domain event as a MediatR notification so it can be published
/// without the Domain layer ever referencing MediatR. Handlers implement
/// INotificationHandler&lt;DomainEventNotification&lt;TEvent&gt;&gt;. See docs/adr/0003.
/// </summary>
public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification
    where TDomainEvent : IDomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}
