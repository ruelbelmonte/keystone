using FluentValidation;
using Keystone.Api.Common;

namespace Keystone.Api.Tests.Common;

public class ValidationExtensionsTests
{
    private const string Key = "existing";

    [Theory]
    [AutoData]
    public void Extract_returns_the_value_the_service_passed_in(string request, int existing)
    {
        var context = new ValidationContext<string>(request);
        context.RootContextData[Key] = existing;

        context.Extract<string, int>(Key).Should().Be(existing);
    }

    [Theory]
    [AutoData]
    public void Extract_returns_default_when_the_key_is_absent(string request)
    {
        var context = new ValidationContext<string>(request);

        context.Extract<string, object>(Key).Should().BeNull();
    }
}
