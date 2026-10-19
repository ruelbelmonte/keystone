namespace Keystone.Contracts;

/// <summary>The running build of the API.</summary>
/// <param name="Version">The assembly informational version.</param>
public sealed record VersionResponse(string Version);
