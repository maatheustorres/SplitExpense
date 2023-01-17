using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Persistence.Configurations;

internal sealed class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ToTable("UserGroup");
        builder.HasNoKey();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .IsRequired();

        builder.HasOne<Group>()
            .WithMany()
            .HasForeignKey(g => g.GroupId)
            .IsRequired();
    }
}
