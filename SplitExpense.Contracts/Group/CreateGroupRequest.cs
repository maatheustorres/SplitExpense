namespace SplitExpense.Contracts.Group;

public sealed class CreateGroupRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
}
