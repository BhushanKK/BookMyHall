using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallFavouriteConfiguration : IEntityTypeConfiguration<HallFavourite>
{
    public void Configure(EntityTypeBuilder<HallFavourite> builder)
    {
        builder.ToTable("HallFavourite", "review");
        builder.HasKey(x =>  x.HallFavouriteId );
        builder.Property(x => x.HallFavouriteId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
