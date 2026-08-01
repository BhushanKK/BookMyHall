using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;

namespace BookMyHall.Application.Abstractions.Persistence.Repositories;

public interface IFoodTypeRepository
{
    Task AddAsync(FoodType foodType, CancellationToken cancellationToken = default);

    Task UpdateAsync(FoodType foodType,CancellationToken cancellationToken = default);

    Task<FoodType?> GetByIdAsync(Guid foodTypeId,CancellationToken cancellationToken = default);

    Task<FoodType?> GetByFoodTypeNameAsync(string foodTypeName,CancellationToken cancellationToken = default);

    Task<PaginatedResult<FoodType>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default);
}