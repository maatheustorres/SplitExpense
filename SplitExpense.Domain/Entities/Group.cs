using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Enumerations;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Domain.Entities;

public sealed class Group : AggregateRoot
{
    private Group(Name name, Category category, DateTime createdOnUtc)
        : base(Guid.NewGuid())
    {
        Name = name;
        Category = category;
        CreatedOnUtc = createdOnUtc;
    }

    private Group() { }

    public Name Name { get; private set; }
    public Category Category { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public IList<UserGroup> UserGroup { get; set; }

    public static Group Create(Name name, Category category)
    {
        return new Group(name, category, DateTime.UtcNow);
    }

    public void ChangeName(Name name)
    {
        Name = name;
    }
}
