using System.Net;
using AutoMapper;
using FluentAssertions;
using Moq;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Application.Features.Identity;
using BookMyHall.Contracts.Common;
using BookMyHall.Shared.Common;
using BookMyHall.Shared.Constants;

using DomainRole = BookMyHall.Domain.Entities.Identity.Role;
using BookMyHall.Application.Abstractions.Caching;

namespace BookMyHall.Application.Tests.Features.Identity.Role;

public sealed class GetRolesQueryHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMessageHelper> _messageHelperMock = new();
    private readonly Mock<ICacheService> _cacheService=new();
    private readonly GetRolesQueryHandler _handler;

    public GetRolesQueryHandlerTests()
    {
        _handler = new GetRolesQueryHandler(
            _roleRepositoryMock.Object,
            _mapperMock.Object,
            _messageHelperMock.Object,
            _cacheService.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnRoles_WhenRolesExist()
    {
        // Arrange
        var request = CreatePaginationRequest();
        var query = new GetRolesQuery(request);

        var roles = new List<DomainRole>
        {
            CreateRole("Admin"),
            CreateRole("Manager")
        };

        var pagedResult = new PaginatedResult<DomainRole>
        {
            Items = roles,
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 2
        };

        _roleRepositoryMock
            .Setup(x => x.GetAllAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        _mapperMock
            .Setup(x => x.Map<IReadOnlyList<DomainRole>>(roles))
            .Returns(roles);

        _messageHelperMock
            .Setup(x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Roles retrieved successfully.");

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Message.Should().Be("Roles retrieved successfully.");

        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().BeEquivalentTo(roles);
        result.Data.PageNumber.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
        result.Data.TotalRecords.Should().Be(2);
        result.Data.TotalPages.Should().Be(1);
        result.Data.HasPreviousPage.Should().BeFalse();
        result.Data.HasNextPage.Should().BeFalse();

        _roleRepositoryMock.Verify(
            x => x.GetAllAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IReadOnlyList<DomainRole>>(roles),
            Times.Once);

        _messageHelperMock.Verify(
            x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyResult_WhenNoRolesExist()
    {
        // Arrange
        var request = CreatePaginationRequest();
        var query = new GetRolesQuery(request);

        var roles = new List<DomainRole>();

        var pagedResult = new PaginatedResult<DomainRole>
        {
            Items = roles,
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 0
        };

        _roleRepositoryMock
            .Setup(x => x.GetAllAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        _mapperMock
            .Setup(x => x.Map<IReadOnlyList<DomainRole>>(roles))
            .Returns(roles);

        _messageHelperMock
            .Setup(x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Roles retrieved successfully.");

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be((int)HttpStatusCode.OK);

        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalRecords.Should().Be(0);
        result.Data.PageNumber.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
        result.Data.TotalPages.Should().Be(0);
        result.Data.HasPreviousPage.Should().BeFalse();
        result.Data.HasNextPage.Should().BeFalse();

        _roleRepositoryMock.Verify(
            x => x.GetAllAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IReadOnlyList<DomainRole>>(roles),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectPaginationMetadata()
    {
        // Arrange
        var request = new PaginationRequest
        {
            PageNumber = 2,
            PageSize = 10
        };

        var query = new GetRolesQuery(request);

        var roles = Enumerable
            .Range(1, 10)
            .Select(x => CreateRole($"Role {x}"))
            .ToList();

        var pagedResult = new PaginatedResult<DomainRole>
        {
            Items = roles,
            PageNumber = 2,
            PageSize = 10,
            TotalCount = 25
        };

        _roleRepositoryMock
            .Setup(x => x.GetAllAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        _mapperMock
            .Setup(x => x.Map<IReadOnlyList<DomainRole>>(roles))
            .Returns(roles);

        _messageHelperMock
            .Setup(x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Roles retrieved successfully.");

        // Act
        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data!.PageNumber.Should().Be(2);
        result.Data.PageSize.Should().Be(10);
        result.Data.TotalRecords.Should().Be(25);
        result.Data.TotalPages.Should().Be(3);
        result.Data.HasPreviousPage.Should().BeTrue();
        result.Data.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldPassRequestToRepository()
    {
        // Arrange
        var request = new PaginationRequest
        {
            PageNumber = 3,
            PageSize = 20
        };

        var query = new GetRolesQuery(request);

        var pagedResult = new PaginatedResult<DomainRole>
        {
            Items = [],
            PageNumber = 3,
            PageSize = 20,
            TotalCount = 0
        };

        _roleRepositoryMock
            .Setup(x => x.GetAllAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        _mapperMock
            .Setup(x => x.Map<IReadOnlyList<DomainRole>>(
                It.IsAny<IReadOnlyList<DomainRole>>()))
            .Returns([]);

        _messageHelperMock
            .Setup(x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Roles retrieved successfully.");

        // Act
        await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        _roleRepositoryMock.Verify(
            x => x.GetAllAsync(
                request,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var request = CreatePaginationRequest();
        var query = new GetRolesQuery(request);

        var cancellationToken = new CancellationTokenSource().Token;

        var pagedResult = new PaginatedResult<DomainRole>
        {
            Items = [],
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 0
        };

        _roleRepositoryMock
            .Setup(x => x.GetAllAsync(
                request,
                cancellationToken))
            .ReturnsAsync(pagedResult);

        _mapperMock
            .Setup(x => x.Map<IReadOnlyList<DomainRole>>(
                It.IsAny<IReadOnlyList<DomainRole>>()))
            .Returns([]);

        _messageHelperMock
            .Setup(x => x.RetrievedEntity(
                ResourceNames.Entities,
                EntityKeys.Role))
            .Returns("Roles retrieved successfully.");

        // Act
        await _handler.Handle(
            query,
            cancellationToken);

        // Assert
        _roleRepositoryMock.Verify(
            x => x.GetAllAsync(
                request,
                cancellationToken),
            Times.Once);
    }

    private static PaginationRequest CreatePaginationRequest()
    {
        return new PaginationRequest
        {
            PageNumber = 1,
            PageSize = 10
        };
    }

    private static DomainRole CreateRole(string roleName)
    {
        return new DomainRole
        {
            RoleName = roleName,
            IsActive = true
        };
    }
}