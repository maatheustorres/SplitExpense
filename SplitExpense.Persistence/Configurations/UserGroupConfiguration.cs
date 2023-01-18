using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Persistence.Configurations;

internal sealed class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ToTable("UserGroup");
        builder.HasKey(x => x.Id);

        builder.HasOne<User>(x => x.User)
            .WithMany(x => x.UserGroup)
            .HasForeignKey(u => u.UserId)
            .HasConstraintName("FK_USER_GROUP")
            .IsRequired();

        builder.HasOne<Group>(x => x.Group)
            .WithMany(x => x.UserGroup)
            .HasForeignKey(g => g.GroupId)
            .HasConstraintName("FK_GROUP_USER")
            .IsRequired();
    }
}
