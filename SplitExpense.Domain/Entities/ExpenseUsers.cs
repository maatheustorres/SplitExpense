using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Domain.Entities;

public sealed class ExpenseUsers : AggregateRoot
{
    public ExpenseUsers(decimal payTo, User user, Expense expense)
        : base(Guid.NewGuid())
    {
        PayTo = payTo;
        UserId = user.Id;
        ExpenseId = expense.Id;
    }

    private ExpenseUsers() { }

    public decimal PayTo { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ExpenseId { get; private set; }

    public static ExpenseUsers AddUsersToExpense(PayTo pay, User user, Expense expense)
    {
        return new ExpenseUsers(pay, user, expense);
    }

    public void Update(decimal splitValueToPay)
    {
        PayTo = splitValueToPay;
    }
}
