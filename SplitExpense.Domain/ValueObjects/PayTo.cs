using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Domain.ValueObjects;

public sealed class PayTo : ValueObject
{
    public const int MinimunExpense = 1;
    public PayTo(decimal value) => Value = value;

    public decimal Value { get; }

    public static implicit operator decimal(PayTo payTo) => payTo.Value;

    public static ResultT<PayTo> Create(decimal totalExpense)
    {
        if (totalExpense < MinimunExpense)
        {
            return Result.Failure<PayTo>(DomainErrors.Expense.InvalidExpense);
        }

        return new PayTo(totalExpense);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
