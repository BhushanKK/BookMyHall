using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBookHall.Domain.Identity;

namespace BookMyHall.Persistence.Context;
public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken", "identity");
        builder.HasKey(x =>  x.RefreshTokenId );
        builder.Property(x => x.RefreshTokenId).HasDefaultValueSql("gen_random_uuid()");;
    }
}