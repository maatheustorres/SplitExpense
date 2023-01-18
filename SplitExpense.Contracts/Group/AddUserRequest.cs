namespace SplitExpense.Contracts.Group;

public sealed class AddUserRequest
{
    public Guid GroupId { get; set; }
    public IReadOnlyList<string> Emails { get; set; }
}
