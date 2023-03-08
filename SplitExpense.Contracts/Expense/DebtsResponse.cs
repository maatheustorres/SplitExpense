namespace SplitExpense.Contracts.Expense;

public sealed class DebtsResponse
{
    public decimal PayTo { get; set; }
    public decimal TotalExpense { get; set; }
    public string GroupName { get; set; }
    public string UserToReceive { get; set; }
}
