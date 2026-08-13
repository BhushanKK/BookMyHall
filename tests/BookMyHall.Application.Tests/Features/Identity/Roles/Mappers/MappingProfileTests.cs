using AutoMapper;
using BookMyHall.Application.Common.Mapping;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Domain.Entities.Identity;
using FluentAssertions;

using Microsoft.Extensions.Logging;

namespace BookMyHall.Application.Tests.Common.Mapping;

public sealed class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        using var loggerFactory = LoggerFactory.Create(
            builder => { });

        var configuration = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },
            loggerFactory);

        configuration.AssertConfigurationIsValid();

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Configuration_ShouldBeValid()
    {
        // Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_MapRoleToRoleDto()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var role = new Role
        {
            RoleId = roleId,
            RoleName = "Admin"
        };

        // Act
        var result = _mapper.Map<RoleDto>(role);

        // Assert
        result.Should().NotBeNull();
        result.RoleId.Should().Be(role.RoleId);
        result.RoleName.Should().Be(role.RoleName);
    }

    [Fact]
    public void Should_MapRoleDtoToRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        var roleDto = new RoleDto
        {
            RoleId = roleId,
            RoleName = "Admin"
        };

        // Act
        var result = _mapper.Map<Role>(roleDto);

        // Assert
        result.Should().NotBeNull();
        result.RoleId.Should().Be(roleDto.RoleId);
        result.RoleName.Should().Be(roleDto.RoleName);
    }

    [Fact]
    public void Should_MapRoleWithDifferentValues()
    {
        // Arrange
        var role = new Role
        {
            RoleId = Guid.NewGuid(),
            RoleName = "Manager",
            IsActive = true
        };

        // Act
        var result = _mapper.Map<RoleDto>(role);

        // Assert
        result.Should().BeEquivalentTo(
            new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                IsActive = role.IsActive
            });
    }

    [Fact]
    public void Should_MapRoleDtoWithDifferentValues()
    {
        // Arrange
        var roleDto = new RoleDto
        {
            RoleId = Guid.NewGuid(),
            RoleName = "Customer",
            IsActive = false
        };

        // Act
        var result = _mapper.Map<Role>(roleDto);

        // Assert
        result.Should().BeEquivalentTo(
            new Role
            {
                RoleId = roleDto.RoleId,
                RoleName = roleDto.RoleName,
                IsActive = roleDto.IsActive
            });
    }
}

