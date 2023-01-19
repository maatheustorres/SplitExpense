using SplitExpense.Domain.Core.Primitives;

namespace SplitExpense.Domain.Entities;

public sealed class UserGroup : AggregateRoot
{
    public UserGroup(User user, Group group) 
        : base(Guid.NewGuid())
    {
        UserId = user.Id;
        GroupId = group.Id;
    }

    private UserGroup() { }

    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public Guid GroupId { get; private set; }
    public Group Group { get; private set; }
}
