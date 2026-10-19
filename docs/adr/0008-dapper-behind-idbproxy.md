# 8. Postgres with Dapper behind an IDbProxy seam

Date: 2026-10-20

## Status

Accepted

## Context

EF Core brought migrations and change tracking, but also a transitive SQLite
native package. That package later picked up a high-severity advisory and broke
the build under warnings-as-errors. The services this template is for want SQL
they can read, not a change tracker.

## Decision

- Postgres through Npgsql, with a singleton `NpgsqlDataSource` behind
  `IDbConnectionFactory`.
- Repositories run SQL only through `IDbProxy`, a thin wrapper over Dapper. They
  never call Dapper extension methods or `new` a connection directly, because
  extension methods cannot be substituted in tests.
- Repositories are singletons.
- Uniqueness is enforced by the database (`db/001_create_customers.sql`). The
  validator's existence check gives a friendly 400. A unique violation that loses
  the race is caught in the repository and returned as a 409.

## Consequences

SQL is explicit and the IO boundary is a single interface. The cost is that there
is no migration runner yet: schema scripts in `db/` are applied by hand or by your
deployment pipeline. Adding one (DbUp, or Flyway in CI) is the first thing a real
project should do.
