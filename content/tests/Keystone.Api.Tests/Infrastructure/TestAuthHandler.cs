using System.Security.Claims;
using System.Text.Encodings.Web;
using Keystone.Api.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Keystone.Api.Tests.Infrastructure;

/// <summary>
/// Authenticates every request with all scopes. Send <see cref="ScopesHeader"/> to narrow the
/// scopes, or <see cref="AnonymousHeader"/> to call without credentials.
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    public const string ScopesHeader = "X-Test-Scopes";
    public const string AnonymousHeader = "X-Test-Anonymous";
    public const string Subject = "test-user";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.ContainsKey(AnonymousHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var scopes = Request.Headers.TryGetValue(ScopesHeader, out var header)
            ? header.ToString()
            : $"{Roles.ReadScope} {Roles.WriteScope}";

        var identity = new ClaimsIdentity([new Claim("sub", Subject), new Claim("scope", scopes)], SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
