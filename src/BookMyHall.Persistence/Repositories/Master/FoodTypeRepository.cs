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
         .FirstOrDefaultAsync(x =>x.FoodTypeId == foodTypeId && !x.IsDeleted && x.IsActive,cancellationToken);

    public async Task<FoodType?> GetByFoodTypeNameAsync(string foodTypeName,CancellationToken cancellationToken = default)
        => await context.FoodTypes.AsNoTracking()
            .FirstOrDefaultAsync( x => x.FoodTypeName == foodTypeName &&!x.IsDeleted,cancellationToken);

    public async Task<PaginatedResult<FoodType>> GetAllAsync(PaginationRequest request,CancellationToken cancellationToken = default)
    {
        var query = context.FoodTypes.AsNoTracking().Where(x=>!x.IsDeleted && x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            var pattern=$"%{search}%";
            query = query.Where(x=>EF.Functions.ILike( x.FoodTypeName,pattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.FoodTypeName)
            .ThenBy(x=>x.FoodTypeId)
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
     public async Task<IReadOnlyList<AutoCompleteItem>> GetAutoCompleteAsync(string? searchTerm, 
        int limit = 20, CancellationToken cancellationToken = default)
    {
        var query = context.FoodTypes.AsNoTracking().Where(x => !x.IsDeleted && x.IsActive);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var pattern = $"%{search}%";
            query = query.Where(x => EF.Functions.ILike(x.FoodTypeName, pattern));
        }

        return await query
            .OrderBy(x => x.FoodTypeName)
            .ThenBy(x => x.FoodTypeId)
            .Take(Math.Clamp(limit, 1, 20))
            .Select(x => new AutoCompleteItem(x.FoodTypeId, x.FoodTypeName))
            .ToListAsync(cancellationToken);
    }
}