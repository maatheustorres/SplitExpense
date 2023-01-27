using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Persistence.Configurations;

internal sealed class ExpenseUsersConfiguration : IEntityTypeConfiguration<ExpenseUsers>
{
    public void Configure(EntityTypeBuilder<ExpenseUsers> builder)
    {
        builder.ToTable("ExpenseUsers");
        builder.HasKey(x => x.Id);

        builder.Property(payTo => payTo.PayTo)
                .HasColumnName("PayTo")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .HasConstraintName("FK_USER_EXPENSE")
            .IsRequired();

        builder.HasOne<Expense>()
            .WithMany()
            .HasForeignKey(g => g.ExpenseId)
            .HasConstraintName("FK_EXPENSE_USER")
            .IsRequired();
    }
}
