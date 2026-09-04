using BookMyHall.Domain.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMyHall.Persistence.Context;

public sealed class HallOwnerConfiguration
    : IEntityTypeConfiguration<HallOwnerDto>
{
    public void Configure(EntityTypeBuilder<HallOwnerDto> builder)
    {
        builder.HasNoKey();
        builder.ToView("vw_HallOwners", "identity");
    }
}