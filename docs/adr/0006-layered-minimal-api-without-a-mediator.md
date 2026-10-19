# 6. Layered minimal API without a mediator

Date: 2026-10-20

## Status

Accepted. Supersedes [ADR 2](./0002-cqrs-with-mediatr.md),
[ADR 3](./0003-keep-domain-free-of-mediatr.md) and
[ADR 5](./0005-enforce-layering-with-architecture-tests.md).

## Context

The first version of this template was Clean Architecture with DDD and CQRS over
MediatR. In practice, the services built from it were CRUD-plus-rules APIs. Four
projects, a mediator pipeline and an aggregate model cost more to navigate than
they saved. MediatR was also pinned to a frozen version for licensing reasons.

## Decision

Each feature has one fixed request path, built from plain types:

```
Endpoint (static handler) -> Service -> Validator -> Repository -> IDbProxy
```

- **Minimal APIs only.** Handlers are static methods on a nested `{Resource}Handlers`
  class, never inline lambdas, so their XML docs reach OpenAPI. Binding is
  generated at compile time (`EnableRequestDelegateGenerator`).
- **No MediatR, no AutoMapper, no assembly scanning.** Every registration is
  explicit in `RegisterDependencies` in `Program.cs`. Mapping is hand-written
  `ToXxxResponse()` extensions.
- **Validation lives in validators.** Services validate first and assume valid
  input afterwards.
- **Contracts are a separate project**, so request and response shapes cannot
  depend on entities.

## Consequences

A reader can follow a request through a handful of files, with no runtime
dispatch to trace. Explicit registration means a missing dependency fails at
startup rather than hiding behind a scan.

The cost is that layering is now enforced by convention and review rather than by
architecture tests. The old tests guarded project references, and with the API in
one project there are none left to guard. Cross-cutting behaviour that a mediator
pipeline would add (logging, validation) is written into each service by hand.
That is deliberate, but it is repetition.
