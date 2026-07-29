using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class EventCategoryConfiguration : IEntityTypeConfiguration<EventCategory>
{
    public void Configure(EntityTypeBuilder<EventCategory> builder)
    {
        builder.ToTable("EventCategory", "masters");
        builder.HasKey(x =>  x.EventCategoryId );
        builder.Property(x => x.EventCategoryId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
