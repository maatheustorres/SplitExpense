using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Domain.ValueObjects;

public sealed class TotalExpense : ValueObject
{
    public const int MinimunExpense = 1;
    public TotalExpense(decimal value) => Value = value;

    public decimal Value { get; }

    public static implicit operator decimal(TotalExpense totalExpense) => totalExpense.Value;

    public static ResultT<TotalExpense> Create(decimal totalExpense)
    {
        if (totalExpense < MinimunExpense)
        {
            return Result.Failure<TotalExpense>(DomainErrors.Expense.InvalidExpense);
        }

        return new TotalExpense(totalExpense);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
