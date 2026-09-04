using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;

public sealed class HallViewConfiguration
    : IEntityTypeConfiguration<HallListView>
{
    public void Configure(EntityTypeBuilder<HallListView> builder)
    {
        builder.HasNoKey();
        builder.ToView("HallListView", "venue");
    }
}