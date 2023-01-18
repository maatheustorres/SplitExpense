namespace SplitExpense.Contracts.Group;

public sealed class AddUserResponse
{
    public AddUserResponse(IReadOnlyList<string> emails) => Emails = emails;
    public IReadOnlyList<string> Emails { get; set; }
}
