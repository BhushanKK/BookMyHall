using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Masters;

public sealed class StateTests
{
    [Fact]
    public void State_Should_Be_Inactive_By_Default()
    {
        var state = new State();
        state.IsActive.Should().BeFalse();
    }

    [Fact]
    public void State_Should_Assign_StateId()
    {
        var state = new State();
        var id = Guid.NewGuid();
        state.StateId = id;
        state.StateId.Should().Be(id);
    }

    [Fact]
    public void State_Should_Assign_StateName()
    {
        var state = new State();
        state.StateName = "Maharashtra";
        state.StateName.Should().Be("Maharashtra");
    }

    [Fact]
    public void State_Should_Assign_StateCode()
    {
        var state = new State();
        state.StateCode = "MH";
        state.StateCode.Should().Be("MH");
    }

    [Fact]
    public void State_Should_Assign_IsActive()
    {
        var state = new State();
        state.IsActive = true;
        state.IsActive.Should().BeTrue();
    }

    [Fact]
    public void State_Should_Assign_All_Properties()
    {
        var stateId = Guid.NewGuid();
        var state = new State
        {
            StateId = stateId,
            StateName = "Maharashtra",
            StateCode = "MH",
            IsActive = true
        };

        state.StateId.Should().Be(stateId);
        state.StateName.Should().Be("Maharashtra");
        state.StateCode.Should().Be("MH");
        state.IsActive.Should().BeTrue();
    }

    [Fact]
    public void State_Should_Have_Default_Values()
    {
        var state = new State();
        state.StateId.Should().Be(Guid.Empty);
        state.StateName.Should().BeEmpty();
        state.StateCode.Should().BeEmpty();
        state.IsActive.Should().BeFalse();
    }
}