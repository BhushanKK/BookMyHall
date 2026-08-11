using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookMyHall.Domain.Entities.Identity;

namespace BookMyHall.Persistence.Configurations.Identity;

public sealed class UserPreferenceConfiguration: IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("UserPreference", "identity");

        builder.HasKey(x => x.UserPreferenceId);
    }
}