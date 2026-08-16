using BookMyHall.Domain.Masters;
using FluentAssertions;

namespace BookMyHall.Domain.Tests.Masters;

public sealed class CountryTests
{
    [Fact]
    public void Should_Create_Country_With_Default_Values()
    {
        // Act
        var country = new Country();

        // Assert
        country.CountryId.Should().Be(Guid.Empty);
        country.CountryName.Should().Be(string.Empty);
        country.CountryCode.Should().Be(string.Empty);
        country.PhoneCode.Should().BeNull();
        country.CurrencyCode.Should().BeNull();
        country.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Should_Set_All_Properties_Correctly()
    {
        // Arrange
        var countryId = Guid.NewGuid();

        // Act
        var country = new Country
        {
            CountryId = countryId,
            CountryName = "India",
            CountryCode = "IN",
            PhoneCode = "+91",
            CurrencyCode = "INR",
            IsActive = true
        };

        // Assert
        country.CountryId.Should().Be(countryId);
        country.CountryName.Should().Be("India");
        country.CountryCode.Should().Be("IN");
        country.PhoneCode.Should().Be("+91");
        country.CurrencyCode.Should().Be("INR");
        country.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Should_Allow_Null_Optional_Properties()
    {
        // Act
        var country = new Country
        {
            CountryName = "India",
            CountryCode = "IN",
            PhoneCode = null,
            CurrencyCode = null,
            IsActive = true
        };

        // Assert
        country.PhoneCode.Should().BeNull();
        country.CurrencyCode.Should().BeNull();
    }

    [Fact]
    public void Should_Allow_IsActive_To_Be_Changed()
    {
        // Arrange
        var country = new Country
        {
            CountryId = Guid.NewGuid(),
            CountryName = "India",
            CountryCode = "IN",
            IsActive = true
        };

        // Act
        country.IsActive = false;

        // Assert
        country.IsActive.Should().BeFalse();
    }
}