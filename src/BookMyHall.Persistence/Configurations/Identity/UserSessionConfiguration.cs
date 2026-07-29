using BookMyHall.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSession", "identity");
        builder.HasKey(x =>  x.UserSessionId );
        builder.Property(x => x.UserSessionId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
