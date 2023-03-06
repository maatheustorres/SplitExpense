namespace SplitExpense.Contracts.Expense;

public sealed class ExpenseResponse
{
    public ExpenseResponse(Guid expenseId)
    {
        ExpenseId = expenseId;
    }

    public Guid ExpenseId { get; private set; }
}
