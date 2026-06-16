# keystone

An enterprise-grade Clean Architecture / DDD starter for .NET, packaged as a
`dotnet new` template so you scaffold a fresh, correctly-named solution in one
command. It is opinionated on the things that are expensive to get wrong later:
the dependency rule is enforced by tests, domain events are dispatched after the
transaction commits, failures are typed, and every decision has an ADR.

This is not "yet another clean architecture template". The differentiators are
operability and enforcement: architecture tests that fail the build on a layer
violation, domain-event dispatch wired through persistence, the Result pattern
for expected failures, ProblemDetails error handling, OpenTelemetry, and central
package management, with the reasoning written down in `docs/adr`.

## Use it as a template

```bash
# from this repo
dotnet pack Keystone.TemplatePack.csproj -o ./artifacts
dotnet new install ./artifacts/Keystone.CleanArchitecture.Templates.0.1.0.nupkg

# then, anywhere
dotnet new keystone -n Acme.Billing
cd Acme.Billing
dotnet test
```

`dotnet new keystone -n Acme.Billing` produces a solution with every `Keystone`
renamed to `Acme.Billing`, no git history to strip.

## What's inside

```
content/
  src/
    Keystone.Domain/          entities, value objects, aggregates, domain events (depends on nothing)
    Keystone.Application/      CQRS use cases, validation + logging behaviours, interfaces
    Keystone.Infrastructure/  EF Core, repositories, domain-event dispatch
    Keystone.Api/             minimal API, ProblemDetails, health, OpenTelemetry
  tests/
    Keystone.Domain.Tests/
    Keystone.Application.Tests/
    Keystone.Architecture.Tests/   enforces the dependency rule
```

The dependency rule points inward and the Domain depends on nothing. That is not
a convention here, it is a test (`LayeringTests`).

## Patterns

- **DDD tactical:** `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`,
  strongly-typed ids, domain events raised by aggregates.
- **CQRS** with MediatR (pinned to the last free version, see ADR 0002) and a
  validation + logging pipeline.
- **Result pattern** for expected failures; `ValidationException` to ProblemDetails
  for malformed input (ADR 0004).
- **Domain events** dispatched after `SaveChanges` commits, without the Domain
  depending on MediatR (ADR 0003).
- **Central Package Management** and warnings-as-errors across the solution.
- **OpenTelemetry** tracing and metrics, opt-in OTLP export.

## Build and run

```bash
cd content
dotnet test                              # all unit + architecture tests
dotnet run --project src/Keystone.Api    # POST /customers, GET /customers/{id}, /health
```

## Decisions

See [docs/adr](./docs/adr). Read those first; they are the point of a template
you intend to reuse.
