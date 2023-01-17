namespace SplitExpense.Contracts.Group;

public class GroupResponse
{
    public GroupResponse(Guid id) => Id = id;
    public Guid Id { get; }
}
