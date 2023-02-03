using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Domain.ValueObjects;

public class Name : ValueObject
{
    public const int MaxLength = 100;

    private Name(string value) => Value = value;

    public string Value { get; }

    public static implicit operator string(Name name) => name.Value;

    public static ResultT<Name> Create(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Name>(DomainErrors.Name.NullOrEmpty);
        }

        if(name.Length > MaxLength) 
        {
            return Result.Failure<Name>(DomainErrors.Name.LongerThanAllowed);
        }

        return new Name(name);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
