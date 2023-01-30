namespace SplitExpense.Contracts.Expense;

public sealed class AddUsersToExpenseRequest
{
    public IReadOnlyCollection<Guid> UserIds { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
}
