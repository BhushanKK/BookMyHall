using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Persistence.Context;
public sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetToken", "identity");
        builder.HasKey(x => x.PasswordResetTokenId);
    }
}