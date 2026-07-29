using Npgsql;
using Microsoft.EntityFrameworkCore;
using BookMyHall.Persistence.Context;
using BookMyHall.Persistence.Exceptions;
using BookMyHall.Application.Abstractions.Persistence;

namespace BookMyHall.Persistence;

public sealed class UnitOfWork(BookMyHallDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
            when(ex.InnerException is PostgresException postgresEx &&
                   postgresEx.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new DuplicateRecordException(postgresEx.ConstraintName);
        }
    }
}