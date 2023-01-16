using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Persistence.Configurations;

internal sealed class GroupNameConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(groupName => groupName.Name, nameBuilder =>
        {
            nameBuilder.WithOwner();

            nameBuilder.Property(group => group.Value)
                .HasMaxLength(Name.MaxLength)
                .IsRequired();
        });

        builder.OwnsOne(groupName => groupName.Category, categoryBuilder =>
        {
            categoryBuilder.WithOwner();

            categoryBuilder.Property(group => group.Value)
                .IsRequired();
        });

        builder.Property(groupName => groupName.CreatedOnUtc).IsRequired();
    }
}
