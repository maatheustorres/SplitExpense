using SplitExpense.Domain.Core.Primitives;

namespace SplitExpense.Domain.Entities;

public sealed class Expense : AggregateRoot
{
    public Expense(decimal totalExpense, Guid userId)
        : base(Guid.NewGuid())
    {
        TotalExpense = totalExpense;
        UserId = userId;
    }

    public decimal TotalExpense { get; set; }
    public Guid UserId { get; set; }
}
