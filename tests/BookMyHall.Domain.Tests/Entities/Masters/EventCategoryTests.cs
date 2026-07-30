using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class EventCategoryTests
{
    [Fact]
    public void EventCategory_Should_Be_Inactive_By_Default()
    {
        var eventCategory = new EventCategory();
        eventCategory.IsActive.Should().BeFalse();
    }

    [Fact]
    public void EventCategory_Should_Assign_EventCategoryId()
    {
        var eventCategory = new EventCategory();
        var id = Guid.NewGuid();
        eventCategory.EventCategoryId = id;
        eventCategory.EventCategoryId.Should().Be(id);
    }

    [Fact]
    public void EventCategory_Should_Assign_EventCategoryName()
    {
        var eventCategory = new EventCategory();
        eventCategory.EventCategoryName = "Wedding";
        eventCategory.EventCategoryName.Should().Be("Wedding");
    }

    [Fact]
    public void EventCategory_Should_Assign_IsActive()
    {
        var eventCategory = new EventCategory();
        eventCategory.IsActive = true;
        eventCategory.IsActive.Should().BeTrue();
    }

    [Fact]
    public void EventCategory_Should_Assign_All_Properties()
    {
        var eventCategoryId = Guid.NewGuid();
        var eventCategory = new EventCategory
        {
            EventCategoryId = eventCategoryId,
            EventCategoryName = "Wedding",
            IsActive = true
        };

        eventCategory.EventCategoryId.Should().Be(eventCategoryId);
        eventCategory.EventCategoryName.Should().Be("Wedding");
        eventCategory.IsActive.Should().BeTrue();
    }

    [Fact]
    public void EventCategory_Should_Have_Default_Values()
    {
        var eventCategory = new EventCategory();
        eventCategory.EventCategoryId.Should().Be(Guid.Empty);
        eventCategory.EventCategoryName.Should().BeEmpty();
        eventCategory.IsActive.Should().BeFalse();
    }
}