# 5. Enforce the dependency rule with architecture tests

Date: 2026-06-16

## Status

Superseded by [ADR 6](./0006-layered-minimal-api-without-a-mediator.md)

## Context

Clean Architecture's value comes from the dependency rule: dependencies point
inward, and the Domain depends on nothing. A rule that lives only in a diagram or
a code-review habit erodes the first time someone adds a convenient `using`.

## Decision

The dependency rule is a test. `Keystone.Architecture.Tests` uses NetArchTest to
assert that the Domain has no dependency on outer layers, the Application does not
depend on Infrastructure or the API, and the Infrastructure does not depend on
the API. These run in CI like any other test.

## Consequences

A violation fails the build, so the architecture cannot quietly rot. The cost is
a small test project and keeping the rules current as layers are added. This is
the difference between Clean Architecture as an aspiration and as an enforced
constraint.
