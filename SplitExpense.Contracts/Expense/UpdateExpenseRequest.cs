namespace SplitExpense.Contracts.Expense;

public sealed class UpdateExpenseRequest
{
    public decimal Expense { get; set; }
    public bool Paid { get; set; }
    public Guid UserId { get; set; }
    public Guid UserGroupId { get; set; }
}
