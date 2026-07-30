using FluentAssertions;
using BookMyHall.Domain.Venue;

namespace BookMyHall.Domain.Tests.Venue;

public sealed class HallBlockTests
{
    [Fact]
    public void HallBlock_Should_Be_Inactive_By_Default()
    {
        var hallBlock = new HallBlock();
        hallBlock.IsActive.Should().BeFalse();
    }

    [Fact]
    public void HallBlock_Should_Assign_HallBlockId()
    {
        var hallBlock = new HallBlock();
        var id = Guid.NewGuid();
        hallBlock.HallBlockId = id;
        hallBlock.HallBlockId.Should().Be(id);
    }

    [Fact]
    public void HallBlock_Should_Assign_HallId()
    {
        var hallBlock = new HallBlock();
        var hallId = Guid.NewGuid();
        hallBlock.HallId = hallId;
        hallBlock.HallId.Should().Be(hallId);
    }

    [Fact]
    public void HallBlock_Should_Assign_BlockFromDate()
    {
        var hallBlock = new HallBlock();
        var fromDate = DateTime.UtcNow;
        hallBlock.BlockFromDate = fromDate;
        hallBlock.BlockFromDate.Should().Be(fromDate);
    }

    [Fact]
    public void HallBlock_Should_Assign_BlockToDate()
    {
        var hallBlock = new HallBlock();
        var toDate = DateTime.UtcNow.AddDays(2);
        hallBlock.BlockToDate = toDate;
        hallBlock.BlockToDate.Should().Be(toDate);
    }

    [Fact]
    public void HallBlock_Should_Assign_StartTime()
    {
        var hallBlock = new HallBlock();
        var startTime = new TimeSpan(10, 0, 0);
        hallBlock.StartTime = startTime;
        hallBlock.StartTime.Should().Be(startTime);
    }

    [Fact]
    public void HallBlock_Should_Assign_EndTime()
    {
        var hallBlock = new HallBlock();
        var endTime = new TimeSpan(18, 0, 0);
        hallBlock.EndTime = endTime;
        hallBlock.EndTime.Should().Be(endTime);
    }

    [Fact]
    public void HallBlock_Should_Assign_Reason()
    {
        var hallBlock = new HallBlock();
        hallBlock.Reason = "Maintenance work";
        hallBlock.Reason.Should().Be("Maintenance work");
    }

    [Fact]
    public void HallBlock_Should_Assign_IsActive()
    {
        var hallBlock = new HallBlock();
        hallBlock.IsActive = true;
        hallBlock.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallBlock_Should_Assign_All_Properties()
    {
        var hallBlockId = Guid.NewGuid();
        var hallId = Guid.NewGuid();
        var fromDate = DateTime.UtcNow;
        var toDate = DateTime.UtcNow.AddDays(2);
        var startTime = new TimeSpan(10, 0, 0);
        var endTime = new TimeSpan(18, 0, 0);

        var hallBlock = new HallBlock
        {
            HallBlockId = hallBlockId,
            HallId = hallId,
            BlockFromDate = fromDate,
            BlockToDate = toDate,
            StartTime = startTime,
            EndTime = endTime,
            Reason = "Maintenance work",
            IsActive = true
        };

        hallBlock.HallBlockId.Should().Be(hallBlockId);
        hallBlock.HallId.Should().Be(hallId);
        hallBlock.BlockFromDate.Should().Be(fromDate);
        hallBlock.BlockToDate.Should().Be(toDate);
        hallBlock.StartTime.Should().Be(startTime);
        hallBlock.EndTime.Should().Be(endTime);
        hallBlock.Reason.Should().Be("Maintenance work");
        hallBlock.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallBlock_Should_Have_Default_Values()
    {
        var hallBlock = new HallBlock();
        hallBlock.HallBlockId.Should().Be(Guid.Empty);
        hallBlock.HallId.Should().Be(Guid.Empty);
        hallBlock.BlockFromDate.Should().Be(default);
        hallBlock.BlockToDate.Should().Be(default);
        hallBlock.StartTime.Should().Be(default);
        hallBlock.EndTime.Should().Be(default);
        hallBlock.Reason.Should().BeEmpty();
        hallBlock.IsActive.Should().BeFalse();
    }
}