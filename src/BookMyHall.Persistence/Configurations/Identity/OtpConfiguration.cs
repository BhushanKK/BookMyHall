using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class OtpConfiguration : IEntityTypeConfiguration<OTP>
{
    public void Configure(EntityTypeBuilder<OTP> builder)
    {
        builder.ToTable("OTP", "identity");
        builder.HasKey(x =>  x.OTPId );
        builder.Property(x => x.OTPId).HasDefaultValueSql("gen_random_uuid()");;
    }
}