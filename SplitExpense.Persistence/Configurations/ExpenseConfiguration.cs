using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Persistence.Configurations;

internal sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expense");
        builder.HasKey(x => x.Id);  

        builder.Property(x => x.TotalExpense).HasColumnName("TotalExpense").IsRequired();

        builder.Property(x => x.Paid).HasColumnName("Paid").IsRequired();

        builder.HasOne<UserGroup>()
            .WithMany()
            .HasConstraintName("FK_USER_GROUP_EXPENSE")
            .HasForeignKey(u => u.UserGroupId)
            .IsRequired();
    }
}
