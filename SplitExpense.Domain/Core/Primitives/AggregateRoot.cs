namespace SplitExpense.Domain.Core.Primitives;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid Id)
        : base(Id)
    {
    }

    protected AggregateRoot()
    {
    }
}
