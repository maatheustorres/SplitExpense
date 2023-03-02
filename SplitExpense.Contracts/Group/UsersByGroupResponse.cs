namespace SplitExpense.Contracts.Group;

public sealed class UsersByGroupResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
}
