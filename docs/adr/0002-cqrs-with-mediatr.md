# 2. CQRS with MediatR, pinned to the last free version

Date: 2026-06-16

## Status

Accepted

## Context

The Application layer separates commands from queries and runs each through a
pipeline (validation, logging). MediatR is the de-facto standard for this in
.NET and is instantly familiar to reviewers. However, MediatR moved to a
commercial license: version 12.5.0 is the last release under Apache-2.0, and
13.x onward requires a paid license for commercial use above a revenue threshold.

## Decision

Use MediatR, pinned to 12.5.0 via central package management. The pin is
deliberate, not incidental.

## Consequences

The template uses a familiar, well-understood mediator with no licensing cost.
The cost is that it sits on a version that will not receive new features. If a
future need requires a newer MediatR, the choice becomes: pay for a license, or
swap in a small hand-rolled dispatcher (the pipeline-behaviour shape here would
port directly). The decision is recorded so that trade-off is made with eyes
open rather than by accidentally upgrading.
