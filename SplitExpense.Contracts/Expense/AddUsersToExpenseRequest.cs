namespace SplitExpense.Contracts.Expense;

public sealed class AddUsersToExpenseRequest
{
    public IReadOnlyCollection<Guid> UserIds { get; set; }
    public decimal Pay { get; set; }
}
