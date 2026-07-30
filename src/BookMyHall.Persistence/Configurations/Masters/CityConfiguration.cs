using BookMyHall.Domain.Masters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("City", "masters");
        builder.HasKey(x =>  x.CityId );
        builder.Property(x => x.CityId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
