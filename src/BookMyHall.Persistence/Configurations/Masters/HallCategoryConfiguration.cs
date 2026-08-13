using BookMyHall.Domain.Masters;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallCategoryConfiguration : IEntityTypeConfiguration<HallCategory>
{
    public void Configure(EntityTypeBuilder<HallCategory> builder)
    {
        builder.ToTable("HallCategory", "masters");
        builder.HasKey(x =>  x.HallCategoryId );
    }
}