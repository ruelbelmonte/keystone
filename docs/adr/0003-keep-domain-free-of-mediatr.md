# 3. Keep the Domain free of MediatR

Date: 2026-06-16

## Status

Superseded by [ADR 6](./0006-layered-minimal-api-without-a-mediator.md)

## Context

Domain events need to be published through MediatR. The easy path is to make
`IDomainEvent` inherit `MediatR.INotification`, but that makes the Domain layer
depend on a messaging library, which contradicts the rule that the Domain
depends on nothing (enforced by an architecture test).

## Decision

`IDomainEvent` is a pure marker in the Domain. The Application layer defines a
generic `DomainEventNotification<TDomainEvent> : INotification` wrapper. The EF
Core `SaveChangesAsync` collects domain events from tracked aggregates, and after
the save commits, wraps each in the notification and publishes it. Handlers
implement `INotificationHandler<DomainEventNotification<TEvent>>`.

## Consequences

The Domain stays pure and the architecture test for it keeps passing. The cost is
one small wrapper type and a reflection call to close the generic at dispatch
time. Events are published only after the transaction commits, so a handler never
reacts to a change that was rolled back.
