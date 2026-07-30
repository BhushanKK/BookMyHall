using FluentAssertions;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Domain.Tests.Entities.Master;

public sealed class FoodTypeTests
{
    [Fact]
    public void FoodType_Should_Be_Inactive_By_Default()
    {
        var foodType = new FoodType();
        foodType.IsActive.Should().BeFalse();
    }

    [Fact]
    public void FoodType_Should_Assign_FoodTypeId()
    {
        var foodType = new FoodType();
        var id = Guid.NewGuid();
        foodType.FoodTypeId = id;
        foodType.FoodTypeId.Should().Be(id);
    }

    [Fact]
    public void FoodType_Should_Assign_FoodTypeName()
    {
        var foodType = new FoodType();
        foodType.FoodTypeName = "Vegetarian";
        foodType.FoodTypeName.Should().Be("Vegetarian");
    }

    [Fact]
    public void FoodType_Should_Assign_IsActive()
    {
        var foodType = new FoodType();
        foodType.IsActive = true;
        foodType.IsActive.Should().BeTrue();
    }

    [Fact]
    public void FoodType_Should_Assign_All_Properties()
    {
        var foodTypeId = Guid.NewGuid();
        var foodType = new FoodType
        {
            FoodTypeId = foodTypeId,
            FoodTypeName = "Vegetarian",
            IsActive = true
        };

        foodType.FoodTypeId.Should().Be(foodTypeId);
        foodType.FoodTypeName.Should().Be("Vegetarian");
        foodType.IsActive.Should().BeTrue();
    }

    [Fact]
    public void FoodType_Should_Have_Default_Values()
    {
        var foodType = new FoodType();
        foodType.FoodTypeId.Should().Be(Guid.Empty);
        foodType.FoodTypeName.Should().BeEmpty();
        foodType.IsActive.Should().BeFalse();
    }
}