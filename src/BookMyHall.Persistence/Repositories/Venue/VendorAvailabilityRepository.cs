using BookMyHall.Application.Abstractions.Persistence.Repositories;
using BookMyHall.Contracts.Common;
using BookMyHall.Domain.Venue;
using BookMyHall.Persistence.Context;
using Microsoft.EntityFrameworkCore;
namespace BookMyHall.Persistence.Repositories;

public sealed class VendorAvailabilityRepository(BookMyHallDbContext context): IVendorAvailabilityRepository
{
    public async Task AddAsync(VendorAvailability vendorAvailability,CancellationToken cancellationToken = default)
    {
        await context.VendorAvailabilities.AddAsync(vendorAvailability,cancellationToken);
    }

    public Task UpdateAsync(VendorAvailability vendorAvailability,CancellationToken cancellationToken = default)
    {
        context.VendorAvailabilities.Update(vendorAvailability);
        return Task.CompletedTask;
    }

    public async Task<VendorAvailability?> GetByIdAsync(Guid vendorAvailabilityId,CancellationToken cancellationToken = default)
    {
        return await context.VendorAvailabilities
            .FirstOrDefaultAsync(x =>x.VendorAvailabilityId == vendorAvailabilityId,cancellationToken);
    }

    public async Task<bool> ExistsOverlappingAsync(Guid vendorId,short? dayOfWeek,DateOnly? availableDate,
        TimeOnly? startTime,TimeOnly? endTime,Guid? excludeVendorAvailabilityId,CancellationToken cancellationToken = default)
    {
        if (!startTime.HasValue || !endTime.HasValue)
        {
            return false;
        }

        var query = context.VendorAvailabilities
            .Where(x =>x.VendorId == vendorId && x.IsAvailable && x.IsActive);

        if (dayOfWeek.HasValue)
        {
            query = query.Where(x => x.DayOfWeek == dayOfWeek);
        }

        if (availableDate.HasValue)
        {
            query = query.Where(x => x.AvailableDate == availableDate);
        }

        if (excludeVendorAvailabilityId.HasValue)
        {
            query = query.Where(x =>x.VendorAvailabilityId != excludeVendorAvailabilityId.Value);
        }
        return await query.AnyAsync(x => x.StartTime < endTime && x.EndTime > startTime,cancellationToken);
    }

    public async Task<PaginatedResult<VendorAvailability>>GetAllAsync(PaginationRequest request,Guid? vendorId,CancellationToken cancellationToken = default)
    {
        var query = context.VendorAvailabilities
            .AsNoTracking()
            .AsQueryable();

        if (vendorId.HasValue)
        {
            query = query.Where(x => x.VendorId == vendorId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchText =request.SearchText.Trim();

            query = query.Where(x => x.Reason != null && x.Reason.Contains(searchText));
        }

        var totalCounts = await query.CountAsync(cancellationToken);

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "dayofweek" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.DayOfWeek)
                    : query.OrderBy(
                        x => x.DayOfWeek),

            "availabledate" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.AvailableDate)
                    : query.OrderBy(
                        x => x.AvailableDate),

            "starttime" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.StartTime)
                    : query.OrderBy(
                        x => x.StartTime),

            "endtime" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.EndTime)
                    : query.OrderBy(
                        x => x.EndTime),

            "isavailable" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.IsAvailable)
                    : query.OrderBy(
                        x => x.IsAvailable),

            "isactive" =>
                request.SortDescending
                    ? query.OrderByDescending(
                        x => x.IsActive)
                    : query.OrderBy(
                        x => x.IsActive),

            _ =>
                query
                    .OrderBy(x => x.AvailableDate)
                    .ThenBy(x => x.DayOfWeek)
                    .ThenBy(x => x.StartTime)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<VendorAvailability>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCounts
        };
    }
}