using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;
using System.Text.RegularExpressions;

namespace SplitExpense.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public const int MaxLength = 256;

    private const string EmailRegexPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

    private static readonly Lazy<Regex> EmailFormatRegex = 
        new Lazy<Regex>(() => new Regex(EmailRegexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase));

    private Email(string value) => Value = value;

    public string Value { get; }

    public static implicit operator string(Email email) => email.Value;

    public static ResultT<Email> Create(string email)
    {
        if (email is null)
        {
            return Result.Failure<Email>(DomainErrors.Email.NullOrEmpty);
        }

        if (email.Length > MaxLength)
        {
            return Result.Failure<Email>(DomainErrors.Email.LongerThanAllowed);
        }

        if (!EmailFormatRegex.Value.IsMatch(email))
        {
            return Result.Failure<Email>(DomainErrors.Email.InvalidFormat);
        }

        return new Email(email);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
