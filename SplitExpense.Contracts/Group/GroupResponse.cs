namespace SplitExpense.Contracts.Group;

public class GroupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; }
    public Guid UserGroupId { get; set; }
    public Guid UserId { get; set; }
    public string fullname { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}
