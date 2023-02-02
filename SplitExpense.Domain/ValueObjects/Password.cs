using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Domain.ValueObjects;

public sealed class Password : ValueObject
{
    private const int MinPasswordLength = 8;
    private static readonly Func<char, bool> IsLower = c => c is >= 'a' and <= 'z';
    private static readonly Func<char, bool> IsUpper = c => c is >= 'A' and <= 'Z';
    private static readonly Func<char, bool> IsDigit = c => c is >= '0' and <= '9';
    private static readonly Func<char, bool> IsNonAlphaNumeric = c => !(IsLower(c) || IsUpper(c) || IsDigit(c));

    private Password(string value) => Value = value;

    public string Value { get; }

    public static implicit operator string(Password password) => password.Value ?? string.Empty;

    public static ResultT<Password> Create(string password)
    {
        if (string.IsNullOrEmpty(password))
            return Result.Failure<Password>(DomainErrors.Password.NullOrEmpty);

        if (password.Length < MinPasswordLength)
            return Result.Failure<Password>(DomainErrors.Password.TooShort);

        if (!password.Any(IsLower))
            return Result.Failure<Password>(DomainErrors.Password.MissingLowercaseLetter);

        if (!password.Any(IsUpper))
            return Result.Failure<Password>(DomainErrors.Password.MissingUppercaseLetter);

        if (!password.Any(IsDigit))
            return Result.Failure<Password>(DomainErrors.Password.MissingDigit);

        if (!password.Any(IsNonAlphaNumeric))
            return Result.Failure<Password>(DomainErrors.Password.MissingNonAlphaNumeric);

        return new Password(password);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
