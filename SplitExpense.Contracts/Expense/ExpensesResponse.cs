namespace SplitExpense.Contracts.Expense;

public sealed class ExpensesResponse
{
    public ExpensesResponse()
    {
        UserExpense = new List<UserExpenseListResponseModel>();
    }

    public Guid? ExpenseId { get; set; }
    public Guid UserGroupId { get; set; }
    public Guid UserId { get; set; }
    public decimal? TotalExpense { get; set; }
    public bool? Paid { get; set; }
    public string UserToReceive { get; set; }
    public List<UserExpenseListResponseModel> UserExpense { get; set; }

    public sealed class UserExpenseListResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public decimal? Repay { get; set; }
    }
}
