using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
{
    public void Configure(EntityTypeBuilder<LoginHistory> builder)
    {
        builder.ToTable("LoginHistory", "identity");
        builder.HasKey(x =>  x.LoginHistoryId );
    }
}