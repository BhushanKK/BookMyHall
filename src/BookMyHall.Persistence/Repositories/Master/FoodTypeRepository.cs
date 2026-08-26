using Microsoft.EntityFrameworkCore;
using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Masters;
using BookMyHall.Persistence.Context;

namespace BookMyHall.Persistence.Repositories;

public sealed class FoodTypeRepository(BookMyHallDbContext context): IFoodTypeRepository
{
    public async Task AddAsync(FoodType foodType,CancellationToken cancellationToken = default)
        => await context.FoodTypes.AddAsync(foodType,cancellationToken);

    public Task UpdateAsync(FoodType foodType,CancellationToken cancellationToken = default)
    {
        context.FoodTypes.Update(foodType);
        return Task.CompletedTask;
    }

    public async Task<FoodType?> GetByIdAsync(Guid foodTypeId,CancellationToken cancellationToken = default)
        => await context.FoodTypes
        .Where(x=>x.IsDeleted==false)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.FoodTypeId == foodTypeId,
                cancellationToken);

    public async Task<FoodType?> GetByFoodTypeNameAsync(string foodTypeName,CancellationToken cancellationToken = default)
        => await context.FoodTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.FoodTypeName == foodTypeName,
                cancellationToken);

    public async Task<PaginatedResult<FoodType>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        IQueryable<FoodType> query = context.FoodTypes
        .Where(x=>x.IsDeleted==false)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.FoodTypeName,
                    $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.FoodTypeName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<FoodType>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}