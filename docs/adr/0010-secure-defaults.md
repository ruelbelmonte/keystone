# 10. Secure defaults for sensitive data

Date: 2026-10-20

## Status

Accepted

## Context

Services generated from this template may handle personal or health data. A
setting that is safe only if someone remembers to change it is, in practice, not
safe.

## Decision

- **Authenticated by default.** A fallback authorization policy requires a
  signed-in caller on every route. Only `/version`, `/health` and the
  Development-only OpenAPI document are anonymous.
- **Scopes, not claims renaming.** JWT bearer runs with `MapInboundClaims = false`,
  and the `Readers`/`Writers` policies check the space-delimited `scope` claim.
- **No personal data in logs.** Request logging records the method, the path and
  the status, never bodies or query strings. Services log ids, not names or
  emails.
- **Reads and writes of personal data are audited.** Services log an
  `Audit:` event with the record id, the caller's `sub` and `client_id`, and the
  trace id (from SerilogTracing), never the data itself.
- **No internals in errors.** Exception messages are never returned. A 500
  carries only a trace id for correlation.

## Consequences

The template is closed by default and has to be opened deliberately. These
defaults do not amount to compliance with HIPAA, the NZ Health Information
Privacy Code, or anything else. Encryption at rest, retention and access
reviews are still each project's responsibility. So is the audit sink: audit events go to the same Serilog
pipeline as operational logs, and a real system should route them to a
separate, tamper-evident store. A multi-tenant system must also scope every
query to the caller's tenant claim, which this single-tenant slice does not.
