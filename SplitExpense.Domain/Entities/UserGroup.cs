using SplitExpense.Domain.Core.Primitives;

namespace SplitExpense.Domain.Entities;

public sealed class UserGroup : AggregateRoot
{
    public UserGroup() 
        : base(Guid.NewGuid())
    {
    }

    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }
}
