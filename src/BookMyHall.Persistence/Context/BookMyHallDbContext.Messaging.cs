using Microsoft.EntityFrameworkCore;
using BookMyHall.Domain.Outbox;

namespace BookMyHall.Persistence.Context;

public partial class BookMyHallDbContext
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}