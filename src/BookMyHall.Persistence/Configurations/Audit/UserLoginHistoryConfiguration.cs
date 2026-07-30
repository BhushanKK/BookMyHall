using BookMyHall.Domain.Audit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;
public sealed class UserLoginHistoryConfiguration : IEntityTypeConfiguration<UserLoginHistory>
{
    public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
    {
        builder.ToTable("UserLoginHistory", "identity");
        builder.HasKey(x =>  x.UserLoginHistoryId );
        builder.Property(x => x.UserLoginHistoryId).HasDefaultValueSql("gen_random_uuid()");;
    }
}
