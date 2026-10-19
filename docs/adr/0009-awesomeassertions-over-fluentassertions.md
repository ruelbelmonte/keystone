# 9. AwesomeAssertions instead of FluentAssertions

Date: 2026-10-20

## Status

Accepted

## Context

FluentAssertions 8 and later are commercially licensed (Xceed). Version 7.2 is
the last Apache-2.0 release and will not be fixed. This is the same trap as
MediatR in ADR 2: a template should not pass a licence obligation on to every
project generated from it.

## Decision

Use AwesomeAssertions, the Apache-2.0 community fork that keeps the
FluentAssertions API (`.Should()`).

## Consequences

Tests read the same as FluentAssertions tests and carry no licence cost. The risk
is depending on a community fork. If it stalls, the API is close enough to move
back to FluentAssertions (with a licence) or to Shouldly.
