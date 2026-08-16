using BookMyHall.Domain.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class FoodTypeConfiguration : IEntityTypeConfiguration<FoodType>
{
    public void Configure(EntityTypeBuilder<FoodType> builder)
    {
        builder.ToTable("FoodType", "masters");
        builder.HasKey(x =>  x.FoodTypeId );
    }
}
