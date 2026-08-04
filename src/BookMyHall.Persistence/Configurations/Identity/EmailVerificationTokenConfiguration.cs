using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Persistence.Context;
public sealed class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.ToTable("EmailVerificationToken", "identity");
        builder.HasKey(x => x.EmailVerificationTokenId);
    }
}