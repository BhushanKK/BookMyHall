using BookMyHall.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;

public sealed class UserDetailsViewConfiguration: IEntityTypeConfiguration<UserDetailsView>
{
    public void Configure(EntityTypeBuilder<UserDetailsView> builder)
    {
        builder.HasNoKey();
        builder.ToView("UserDetailsView", "identity");
    }
}