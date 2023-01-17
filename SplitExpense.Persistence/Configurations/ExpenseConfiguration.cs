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

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .IsRequired();
    }
}
