using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Domain.ValueObjects;

public sealed class LastName : ValueObject
{
    public const int MaxLength = 100;

    private LastName(string value) => Value = value;

    public string Value { get; private set; }

    public static implicit operator string(LastName lastName) => lastName.Value;

    public static ResultT<LastName> Create(string lastName)
    {
        if (lastName is null)
        {
            return Result.Failure<LastName>(DomainErrors.FirstName.NullOrEmpty);
        }

        if (lastName.Length > MaxLength)
        {
            return Result.Failure<LastName>(DomainErrors.FirstName.LongerThanAllowed);
        }

        return new LastName(lastName);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
