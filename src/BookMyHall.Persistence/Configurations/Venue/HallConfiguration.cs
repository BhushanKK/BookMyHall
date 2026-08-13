using BookMyHall.Domain.Venue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class HallConfiguration : IEntityTypeConfiguration<Hall>
{
    public void Configure(EntityTypeBuilder<Hall> builder)
    {
        builder.ToTable("Hall", "venue");
        builder.HasKey(x =>  x.HallId );

         // Enum -> VARCHAR
        builder.Property(x => x.ApprovalStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Enum -> VARCHAR
        builder.Property(x => x.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
