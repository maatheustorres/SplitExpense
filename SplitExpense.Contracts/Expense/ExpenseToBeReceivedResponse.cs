namespace SplitExpense.Contracts.Expense;

public sealed class ExpenseToBeReceivedResponse
{
    public Guid GroupId { get; set; }
    public Guid UserGroupId { get; set; }
    public string GroupName { get; set; }
    public string Username { get; set; }
    public decimal TotalExpense { get; set; }
}
