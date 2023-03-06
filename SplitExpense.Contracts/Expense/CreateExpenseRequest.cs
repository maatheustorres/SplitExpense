namespace SplitExpense.Contracts.Expense;

public sealed class CreateExpenseRequest
{
    public decimal TotalExpense { get; set; }
    public bool Paid { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
}
