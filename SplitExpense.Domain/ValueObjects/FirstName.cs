using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Domain.ValueObjects;

public sealed class FirstName : ValueObject
{
    public const int MaxLength = 100;

    private FirstName(string value) => Value = value;

    public string Value { get; }

    public static implicit operator string(FirstName firstName) => firstName.Value;

    public static ResultT<FirstName> Create(string firstName)
    {
        if(firstName is null)
        {
            return Result.Failure<FirstName>(DomainErrors.FirstName.NullOrEmpty)
        }

        if(firstName.Length > MaxLength)
        {
            return Result.Failure<FirstName>(DomainErrors.FirstName.LongerThanAllowed);
        } 

        return new FirstName(firstName);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
