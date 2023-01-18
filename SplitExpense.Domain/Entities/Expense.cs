using SplitExpense.Domain.Core.Primitives;

namespace SplitExpense.Domain.Entities;

public sealed class Expense : AggregateRoot
{
    public Expense(decimal totalExpense, bool paid, Guid userId, Guid groupId)
        : base(Guid.NewGuid())
    {
        TotalExpense = totalExpense;
        Paid = paid;
        UserId = userId;
        GroupId = groupId;
    }

    public decimal TotalExpense { get; private set; }
    public bool Paid { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GroupId { get; private set; }

    public static Expense Create(decimal totalExpense, bool paid, Guid userId, Guid groupId) =>
        new(totalExpense, paid, userId, groupId);
}
