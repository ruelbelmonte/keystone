using Microsoft.AspNetCore.Authorization;

namespace Keystone.Api.Common;

/// <summary>Authorization policy names, granted by OAuth scopes on the caller's token.</summary>
public static class Roles
{
    public const string Readers = "Readers";
    public const string Writers = "Writers";

    public const string ReadScope = "keystone.read";
    public const string WriteScope = "keystone.write";

    public static AuthorizationBuilder AddRolePolicies(this AuthorizationBuilder builder) =>
        builder
            .AddPolicy(Readers, policy => policy.RequireAssertion(context => HasAnyScope(context, ReadScope, WriteScope)))
            .AddPolicy(Writers, policy => policy.RequireAssertion(context => HasAnyScope(context, WriteScope)));

    // The scope claim is one space-delimited string, so RequireClaim's exact match would miss it.
    private static bool HasAnyScope(AuthorizationHandlerContext context, params string[] scopes) =>
        context.User.FindAll("scope")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Intersect(scopes, StringComparer.Ordinal)
            .Any();
}
