using System.Reflection;
using Keystone.Contracts;

namespace Keystone.Api.Common;

public static class VersionEndpoints
{
    public static IEndpointRouteBuilder MapVersionEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/version", VersionHandlers.GetVersion)
            .Produces<VersionResponse>()
            .AllowAnonymous();

        return routes;
    }

    internal static class VersionHandlers
    {
        private static readonly VersionResponse Version = new(
            typeof(VersionEndpoints).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown");

        /// <summary>Returns the running build of the API.</summary>
        /// <remarks>Unversioned and anonymous, for deploy checks.</remarks>
        /// <returns>200 with the version.</returns>
        public static IResult GetVersion() => Results.Ok(Version);
    }
}
