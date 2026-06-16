# 4. Result pattern for expected failures; exceptions for the exceptional

Date: 2026-06-16

## Status

Accepted

## Context

Operations fail in two different ways. Some failures are part of normal
operation (a business rule rejects the request, a record is not found). Others
mean the request was malformed or something genuinely unexpected happened.

## Decision

Expected outcomes return `Result` / `Result<T>` with a typed `Error`, so the
failure is visible in the signature and travels without an exception. Input
validation throws a `ValidationException` (the request was malformed), which the
API maps to a 400 ProblemDetails. The API maps a failed `Result` to a status
code from its error (not-found to 404, otherwise 400).

## Consequences

Business logic reads linearly without try/catch for ordinary outcomes, and the
failure modes are explicit. The cost is the discipline of checking results and
maintaining the one place (`ResultExtensions`) that maps errors to status codes.
The split between "Result for business outcomes, exception for malformed input"
is a deliberate line, not an inconsistency.
