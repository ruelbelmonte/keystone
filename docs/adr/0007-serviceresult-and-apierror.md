# 7. ServiceResult and ApiError for expected failures

Date: 2026-10-20

## Status

Accepted. Supersedes [ADR 4](./0004-result-pattern-for-expected-failures.md).

## Context

ADR 4 split failures into Result values for business outcomes and a
`ValidationException` for malformed input. It also mapped errors to status codes
by naming convention (`EndsWith("NotFound")`), which was fragile.

## Decision

- Every service method returns `ServiceResult` or `ServiceResult<T>`. Validation
  failures are expected outcomes too, so they travel as a result, not an exception.
- An `ApiError` carries its own `HttpStatusCode`. Handlers derive the response
  status from the error and never hardcode it:
  `Results.Json(result.Error, ..., statusCode: (int)result.Error.HttpStatusCode)`.
  `Results.BadRequest(error)` is not used, because it would turn a 404 or 409
  into a 400.
- `ApiErrors` is the single registry of codes and statuses.
- Exceptions are only for the exceptional. `GlobalExceptionHandler` maps
  `BadHttpRequestException` to its 4xx, anything else to 500, and returns
  ProblemDetails with a trace id.

## Consequences

The status code an endpoint returns is decided where the failure is understood,
and the registry keeps codes consistent. Clients see two error shapes: `ApiError`
for service failures, and ProblemDetails for malformed requests and 500s.
`ProducesStandardErrorResponses()` documents both.
