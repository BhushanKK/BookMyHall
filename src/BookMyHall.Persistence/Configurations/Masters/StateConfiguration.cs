using BookMyHall.Domain.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BookMyHall.Persistence.Context;
public sealed class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("State", "masters");
        builder.HasKey(x =>  x.StateId );
        builder.Property(x => x.StateId).HasDefaultValueSql("gen_random_uuid()");;
    }
}