using System.Reflection;
using Keystone.Domain.Customers;
using NetArchTest.Rules;

namespace Keystone.Architecture.Tests;

/// <summary>
/// The Clean Architecture dependency rule, enforced as tests rather than left to
/// discipline: dependencies point inward, and the Domain depends on nothing.
/// </summary>
public class LayeringTests
{
    private const string Application = "Keystone.Application";
    private const string Infrastructure = "Keystone.Infrastructure";
    private const string Api = "Keystone.Api";

    private static readonly Assembly DomainAssembly = typeof(Customer).Assembly;
    private static readonly Assembly ApplicationAssembly =
        typeof(Keystone.Application.DependencyInjection).Assembly;
    private static readonly Assembly InfrastructureAssembly =
        typeof(Keystone.Infrastructure.DependencyInjection).Assembly;

    [Fact]
    public void Domain_has_no_dependency_on_outer_layers()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Application, Infrastructure, Api)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_does_not_depend_on_infrastructure_or_api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Infrastructure, Api)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_does_not_depend_on_api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Api)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult result) =>
        "Offending types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
