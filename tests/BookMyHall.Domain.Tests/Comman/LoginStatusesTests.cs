using BookMyHall.Domain.Common;

namespace BookMyHall.Domain.Tests.Common;

public sealed class LoginStatusesTests
{
    [Fact]
    public void LoginStatuses_Success_ShouldHaveCorrectValue()
    {
        Assert.Equal("Success", LoginStatuses.Success);
    }

    [Fact]
    public void LoginStatuses_Failed_ShouldHaveCorrectValue()
    {
        Assert.Equal("Failed", LoginStatuses.Failed);
    }

    [Fact]
    public void LoginStatuses_Locked_ShouldHaveCorrectValue()
    {
        Assert.Equal("Locked", LoginStatuses.Locked);
    }

    [Fact]
    public void LoginStatuses_LoggedOut_ShouldHaveCorrectValue()
    {
        Assert.Equal("LoggedOut", LoginStatuses.LoggedOut);
    }
}