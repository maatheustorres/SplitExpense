namespace SplitExpense.Contracts.SplitExpense;

public sealed class SplitExpenseRequest
{
    public IReadOnlyCollection<Guid> UserIds { get; set; }
    public Guid UserId { get; set; }
}
