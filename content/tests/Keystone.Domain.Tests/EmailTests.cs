using Keystone.Domain.Customers;

namespace Keystone.Domain.Tests;

public class EmailTests
{
    [Fact]
    public void Create_accepts_a_valid_address()
    {
        var result = Email.Create("user@example.com");
        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("no-at-sign")]
    public void Create_rejects_an_invalid_address(string input)
    {
        var result = Email.Create(input);
        Assert.True(result.IsFailure);
        Assert.Equal(CustomerErrors.InvalidEmail, result.Error);
    }

    [Fact]
    public void Addresses_are_normalised_and_compared_by_value()
    {
        var a = Email.Create("Ada@Example.com").Value;
        var b = Email.Create("ada@example.com").Value;
        Assert.Equal(a, b);
    }
}
