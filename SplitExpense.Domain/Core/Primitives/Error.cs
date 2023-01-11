namespace SplitExpense.Domain.Core.Primitives;

public sealed class Error : ValueObject 
{
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    public static implicit operator string(Error error) => error?.Code ?? string.Empty;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
        yield return Message;
    }

    /// <summary>
    /// Gets the empty error instance.
    /// </summary>
    internal static Error None => new Error(string.Empty, string.Empty);
}
