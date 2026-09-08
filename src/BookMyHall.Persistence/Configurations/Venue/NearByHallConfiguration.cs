using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;

public sealed class NearByHallConfiguration
    : IEntityTypeConfiguration<NearbyHallView>
{
    public void Configure(EntityTypeBuilder<NearbyHallView> builder)
    {
        builder.HasNoKey();
        builder.ToView("NearbyHallView", "venue");
    }
}