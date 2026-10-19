# keystone

An opinionated .NET 10 Web API template, packaged for `dotnet new`. It is the
shape I start new services from: every request takes the same path through
plain, explicitly registered types, and every endpoint ships with system tests.

```
Endpoint (static handler) -> Service -> Validator -> Repository -> IDbProxy -> Postgres
```

## Use it as a template

```bash
# from this repo
dotnet pack Keystone.TemplatePack.csproj -o ./artifacts
dotnet new install ./artifacts/Keystone.WebApi.Templates.0.2.0.nupkg

# then, anywhere
dotnet new keystone -n Acme.Billing
cd Acme.Billing
dotnet test
```

Every `Keystone` is renamed to `Acme.Billing`, and there is no git history to
strip. CI generates a project from the packed template and tests it on every push
and once a week.

## What's inside

```
content/
  src/
    Keystone.Contracts/         request/response DTOs and ApiError (no dependencies)
    Keystone.Api/
      Common/                   ServiceResult, ApiErrors, endpoint + validation extensions,
                                GlobalExceptionHandler, Roles policies, /version
      Infrastructure/Data/      IDbProxy (Dapper), IDbConnectionFactory (Npgsql)
      Customers/                the reference vertical slice
        Endpoints/  Services/  Validators/  Repositories/
  tests/
    Keystone.Api.Tests/         system tests over an in-memory host
  db/                           schema scripts
```

`Customers` is the slice to copy. `POST /v1/customers` and
`GET /v1/customers/{id}` go through every layer and have system tests covering
201, 200, 400, 401, 403, 404 and 409.

## The rules

- **Minimal APIs only.** One `Map{Resource}Endpoints` per resource. Handlers are
  static methods on a nested `{Resource}Handlers` class, never lambdas, so their
  XML docs reach OpenAPI. Binding is compile-time generated.
- **Handlers hold no logic.** They make one service call and derive the status
  from the error: `Results.Json(result.Error, ..., statusCode: (int)result.Error.HttpStatusCode)`.
- **Services return `ServiceResult` / `ServiceResult<T>`** and validate first.
  Expected failures are an `ApiError` from the `ApiErrors` registry, never an
  exception.
- **All validation lives in FluentValidation validators.** There are three
  patterns:
  1. Field rules.
  2. A `MustAsync` existence check through a repository.
  3. Data the service already fetched, passed via `RootContextData` and read with
     `Extract<TRequest, T>(key)`.
- **Repositories are singletons and only run SQL through `IDbProxy`**, so tests
  can substitute them.
- **No MediatR, no AutoMapper, no assembly scanning.** Dependencies are
  registered in one `RegisterDependencies` function in `Program.cs`.
- **Central package management** with pinned versions, warnings as errors and
  NuGet audit on.

## Tests

`ApiWebApplicationFactory` hosts the API in memory and replaces every IO
dependency with an NSubstitute substitute that it exposes as a property.
`TestAuthHandler` authenticates with all scopes. Send `X-Test-Scopes` to narrow
them, or `X-Test-Anonymous` to call without credentials.

There is one test class per endpoint method (`PostCustomerTests`), and test names
start with the handler name (`RegisterCustomer_returns_409_when_...`). Tests use
xunit v3, AutoFixture `[InlineAutoData]` and AwesomeAssertions.

## Security defaults

- Every route requires an authenticated caller unless it opts out.
- The `Readers` and `Writers` policies check the token's `scope` claim.
- Logs carry ids, never names, emails, request bodies or query strings.
- Error responses never include exception messages.

These are defaults, not compliance. See [ADR 10](./docs/adr/0010-secure-defaults.md)
for what they leave to each project.

## Run it locally

```bash
docker run -d --name keystone-db -p 5432:5432 \
  -e POSTGRES_USER=keystone -e POSTGRES_PASSWORD=keystone -e POSTGRES_DB=keystone postgres:17
docker exec -i keystone-db psql -U keystone -d keystone < content/db/001_create_customers.sql

cd content
dotnet run --project src/Keystone.Api   # /version, /health, /openapi/v1.json, /v1/customers
```

Customer endpoints need a JWT from the authority configured under
`Authentication:Schemes:Bearer` in `appsettings.json`, carrying the
`keystone.read` or `keystone.write` scope.

## Not included yet

- **A migration runner.** Schema scripts in `db/` are applied by hand.
- **A cache.** For read-heavy paths, add a cache-aside base class with
  `SemaphoreSlim` stampede protection, plus a central cache-key helper.
- **Integration tests against a real Postgres.** System tests substitute the
  repositories, so the SQL itself is untested here. Testcontainers is the
  natural next step.

## Decisions

See [docs/adr](./docs/adr). ADRs 2 to 5 record the earlier Clean Architecture /
DDD design and are kept, marked superseded, so the reasoning for the change is
visible.
