using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Masters;

public sealed class HallCategoryTests
{
    [Fact]
    public void HallCategory_Should_Be_Inactive_By_Default()
    {
        var category = new HallCategory();
        category.IsActive.Should().BeFalse();
    }

    [Fact]
    public void HallCategory_Should_Assign_HallCategoryId()
    {
        var category = new HallCategory();
        var id = Guid.NewGuid();
        category.HallCategoryId = id;
        category.HallCategoryId.Should().Be(id);
    }

    [Fact]
    public void HallCategory_Should_Assign_HallCategoryName()
    {
        var category = new HallCategory();
        category.HallCategoryName = "Wedding Hall";
        category.HallCategoryName.Should().Be("Wedding Hall");
    }

    [Fact]
    public void HallCategory_Should_Assign_IsActive()
    {
        var category = new HallCategory();
        category.IsActive = true;
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallCategory_Should_Assign_All_Properties()
    {
        var categoryId = Guid.NewGuid();
        var category = new HallCategory
        {
            HallCategoryId = categoryId,
            HallCategoryName = "Banquet Hall",
            IsActive = true
        };

        category.HallCategoryId.Should().Be(categoryId);
        category.HallCategoryName.Should().Be("Banquet Hall");
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void HallCategory_Should_Have_Default_Values()
    {
        var category = new HallCategory();
        category.HallCategoryId.Should().Be(Guid.Empty);
        category.HallCategoryName.Should().BeEmpty();
        category.IsActive.Should().BeFalse();
    }
}