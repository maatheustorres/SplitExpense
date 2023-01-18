using SplitExpense.Domain.Core.Primitives;

namespace SplitExpense.Domain.Entities;

public sealed class Expense : AggregateRoot
{
    public Expense(decimal totalExpense, bool paid, Guid userGroupId)
        : base(Guid.NewGuid())
    {
        TotalExpense = totalExpense;
        Paid = paid;
        UserGroupId = userGroupId;
    }

    public decimal TotalExpense { get; private set; }
    public bool Paid { get; private set; }
    public Guid UserGroupId { get; private set; }

    public static Expense Create(decimal totalExpense, bool paid, Guid userGroupId) =>
        new(totalExpense, paid, userGroupId);
}
