// using AutoMapper;
// using BookMyHall.Application.Abstractions.Persistence.Repositories;
// using BookMyHall.Application.Features.Identity;
// using BookMyHall.Contracts.Common;
// using BookMyHall.Shared.Common;
// using FluentAssertions;
// using Moq;

// namespace BookMyHall.Application.Tests.Features.Identity;

// public sealed class GetRolesQueryTests
// {
//     private readonly Mock<IRoleRepository> _roleRepositoryMock;
//     private readonly Mock<IMessageHelper> _messageHelperMock;
//     private readonly Mock<IMapper> _mapperMock;
//     private readonly GetRolesQueryHandler _handler;

//     public GetRolesQueryTests()
//     {
//         _roleRepositoryMock = new Mock<IRoleRepository>();
//         _messageHelperMock = new Mock<IMessageHelper>();
//         _mapperMock = new Mock<IMapper>();

//         _handler = new GetRolesQueryHandler(
//             _roleRepositoryMock.Object,
//             _messageHelperMock.Object,
//             _mapperMock.Object);
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnPaginatedRoles_WhenRolesExist()
//     {
//         // Arrange
//         var roles = new List<RoleDto>
//         {
//             new()
//             {
//                 RoleId = Guid.NewGuid(),
//                 RoleName = "Admin"
//             },
//             new()
//             {
//                 RoleId = Guid.NewGuid(),
//                 RoleName = "User"
//             }
//         };

//         var roleDtos = new List<RoleDto>
//         {
//             new()
//             {
//                 RoleId = roles[0].RoleId,
//                 RoleName = roles[0].RoleName
//             },
//             new()
//             {
//                 RoleId = roles[1].RoleId,
//                 RoleName = roles[1].RoleName
//             }
//         };

//         const int pageNumber = 1;
//         const int pageSize = 10;

//         _roleRepositoryMock
//             .Setup(x => x.GetAllAsync(It.IsAny<PaginationRequest>(),
//                 It.IsAny<CancellationToken>())).ReturnsAsync((roles, 2L));

//         _mapperMock
//             .Setup(x => x.Map<List<RoleDto>>(roles))
//             .Returns(roleDtos);

//         var query = new GetRolesQuery(
//             new PaginationRequest
//             {
//                 PageNumber = pageNumber,
//                 PageSize = pageSize
//             });

//         // Act
//         var result = await _handler.Handle(
//             query,
//             CancellationToken.None);

//         // Assert
//         result.Should().NotBeNull();
//         result.Success.Should().BeTrue();
//         result.Data.Should().NotBeNull();

//         result.Data!.Items.Should().HaveCount(2);
//         result.Data.Items.Should().BeEquivalentTo(roleDtos);

//         result.Data.PageNumber.Should().Be(pageNumber);
//         result.Data.PageSize.Should().Be(pageSize);
//         result.Data.TotalRecords.Should().Be(2);
//         result.Data.TotalPages.Should().Be(1);
//         result.Data.HasPreviousPage.Should().BeFalse();
//         result.Data.HasNextPage.Should().BeFalse();

//         _roleRepositoryMock.Verify(
//             x => x.GetAllAsync(
//                 It.Is<PaginationRequest>(p =>
//                     p.PageNumber == pageNumber &&
//                     p.PageSize == pageSize),
//                 It.IsAny<CancellationToken>()),
//             Times.Once);

//         _mapperMock.Verify(
//             x => x.Map<List<RoleDto>>(roles),
//             Times.Once);
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnEmptyPaginatedResponse_WhenNoRolesExist()
//     {
//         // Arrange
//         var roles = new List<RoleDto>();
//         var roleDtos = new List<RoleDto>();

//         const int pageNumber = 1;
//         const int pageSize = 10;

//         _roleRepositoryMock
//             .Setup(x => x.GetAllAsync(
//                 It.IsAny<PaginationRequest>(),
//                 It.IsAny<CancellationToken>()))
//             .ReturnsAsync((roles, 0L));

//         _mapperMock
//             .Setup(x => x.Map<List<RoleDto>>(roles))
//             .Returns(roleDtos);

//         var query = new GetRolesQuery(
//             new PaginationRequest
//             {
//                 PageNumber = pageNumber,
//                 PageSize = pageSize
//             });

//         // Act
//         var result = await _handler.Handle(
//             query,
//             CancellationToken.None);

//         // Assert
//         result.Should().NotBeNull();
//         result.Success.Should().BeTrue();
//         result.Data.Should().NotBeNull();

//         result.Data!.Items.Should().BeEmpty();
//         result.Data.PageNumber.Should().Be(pageNumber);
//         result.Data.PageSize.Should().Be(pageSize);
//         result.Data.TotalRecords.Should().Be(0);
//         result.Data.TotalPages.Should().Be(0);
//         result.Data.HasPreviousPage.Should().BeFalse();
//         result.Data.HasNextPage.Should().BeFalse();

//         _roleRepositoryMock.Verify(
//             x => x.GetAllAsync(
//                 It.IsAny<PaginationRequest>(),
//                 It.IsAny<CancellationToken>()),
//             Times.Once);
//     }

//     [Fact]
//     public async Task Handle_ShouldCalculateNextPage_WhenMoreRecordsExist()
//     {
//         // Arrange
//         var roles = Enumerable.Range(1, 10)
//             .Select(_ => new RoleDto
//             {
//                 RoleId = Guid.NewGuid(),
//                 RoleName = $"Role-{Guid.NewGuid()}"
//             })
//             .ToList();

//         var roleDtos = roles
//             .Select(x => new RoleDto
//             {
//                 RoleId = x.RoleId,
//                 RoleName = x.RoleName
//             })
//             .ToList();

//         const int pageNumber = 1;
//         const int pageSize = 10;
//         const long totalRecords = 25;

//         _roleRepositoryMock
//             .Setup(x => x.GetAllAsync(
//                 It.IsAny<PaginationRequest>(),
//                 It.IsAny<CancellationToken>()))
//             .ReturnsAsync((roles, totalRecords));

//         _mapperMock
//             .Setup(x => x.Map<List<RoleDto>>(roles))
//             .Returns(roleDtos);

//         var query = new GetRolesQuery(
//             new PaginationRequest
//             {
//                 PageNumber = pageNumber,
//                 PageSize = pageSize
//             });

//         // Act
//         var result = await _handler.Handle(
//             query,
//             CancellationToken.None);

//         // Assert
//         result.Should().NotBeNull();
//         result.Success.Should().BeTrue();
//         result.Data.Should().NotBeNull();

//         result.Data!.Items.Should().HaveCount(10);
//         result.Data.PageNumber.Should().Be(1);
//         result.Data.PageSize.Should().Be(10);
//         result.Data.TotalRecords.Should().Be(25);

//         result.Data.TotalPages.Should().Be(3);
//         result.Data.HasPreviousPage.Should().BeFalse();
//         result.Data.HasNextPage.Should().BeTrue();
//     }

//     [Fact]
//     public async Task Handle_ShouldSetPreviousAndNextPageCorrectly_WhenOnMiddlePage()
//     {
//         // Arrange
//         var roles = Enumerable.Range(1, 10)
//             .Select(_ => new RoleDto
//             {
//                 RoleId = Guid.NewGuid(),
//                 RoleName = $"Role-{Guid.NewGuid()}"
//             })
//             .ToList();

//         var roleDtos = roles
//             .Select(x => new RoleDto
//             {
//                 RoleId = x.RoleId,
//                 RoleName = x.RoleName
//             })
//             .ToList();

//         const int pageNumber = 2;
//         const int pageSize = 10;
//         const long totalRecords = 25;

//         _roleRepositoryMock
//             .Setup(x => x.GetAllAsync(
//                 It.IsAny<PaginationRequest>(),
//                 It.IsAny<CancellationToken>()))
//             .ReturnsAsync((roles, totalRecords));

//         _mapperMock
//             .Setup(x => x.Map<List<RoleDto>>(roles))
//             .Returns(roleDtos);

//         var query = new GetRolesQuery(
//             new PaginationRequest
//             {
//                 PageNumber = pageNumber,
//                 PageSize = pageSize
//             });

//         // Act
//         var result = await _handler.Handle(
//             query,
//             CancellationToken.None);

//         // Assert
//         result.Should().NotBeNull();
//         result.Data.Should().NotBeNull();

//         result.Data!.TotalPages.Should().Be(3);
//         result.Data.HasPreviousPage.Should().BeTrue();
//         result.Data.HasNextPage.Should().BeTrue();
//     }

//     [Fact]
//     public async Task Handle_ShouldSetOnlyPreviousPage_WhenOnLastPage()
//     {
//         // Arrange
//         var roles = new List<RoleDto>
//         {
//             new()
//             {
//                 RoleId = Guid.NewGuid(),
//                 RoleName = "Admin"
//             },
//             new()
//             {
//                 RoleId = Guid.NewGuid(),
//                 RoleName = "User"
//             }
//         };

//         var roleDtos = new List<RoleDto>
//         {
//             new()
//             {
//                 RoleId = roles[0].RoleId,
//                 RoleName = roles[0].RoleName
//             },
//             new()
//             {
//                 RoleId = roles[1].RoleId,
//                 RoleName = roles[1].RoleName
//             }
//         };

//         const int pageNumber = 3;
//         const int pageSize = 10;
//         const long totalRecords = 22;

//         _roleRepositoryMock
//             .Setup(x => x.GetAllAsync(
//                 It.IsAny<PaginationRequest>(),
//                 It.IsAny<CancellationToken>()))
//             .ReturnsAsync((roles, totalRecords));

//         _mapperMock
//             .Setup(x => x.Map<List<RoleDto>>(roles))
//             .Returns(roleDtos);

//         var query = new GetRolesQuery(
//             new PaginationRequest
//             {
//                 PageNumber = pageNumber,
//                 PageSize = pageSize
//             });

//         // Act
//         var result = await _handler.Handle(
//             query,
//             CancellationToken.None);

//         // Assert
//         result.Should().NotBeNull();
//         result.Data.Should().NotBeNull();

//         result.Data!.Items.Should().HaveCount(2);
//         result.Data.TotalRecords.Should().Be(22);
//         result.Data.TotalPages.Should().Be(3);

//         result.Data.HasPreviousPage.Should().BeTrue();
//         result.Data.HasNextPage.Should().BeFalse();
//     }
// }