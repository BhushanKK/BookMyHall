using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Tests.Common;

public sealed class LoginMethodsTests
{
    [Fact]
    public void LoginMethods_Password_ShouldHaveCorrectValue()
    {
        Assert.Equal("Password", LoginMethods.Password);
    }

    [Fact]
    public void LoginMethods_Google_ShouldHaveCorrectValue()
    {
        Assert.Equal("Google", LoginMethods.Google);
    }

    [Fact]
    public void LoginMethods_Microsoft_ShouldHaveCorrectValue()
    {
        Assert.Equal("Microsoft", LoginMethods.Microsoft);
    }

    [Fact]
    public void LoginMethods_OTP_ShouldHaveCorrectValue()
    {
        Assert.Equal("OTP", LoginMethods.OTP);
    }
}