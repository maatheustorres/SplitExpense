namespace SplitExpense.Domain.Core.Primitives.Result;

public class ResultT<TValue> : Result
{
    private readonly TValue _value;

    protected internal ResultT(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
        => _value = value;

    public static implicit operator ResultT<TValue>(TValue value) => Success(value);

    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("The value of a failure result can not be accessed");
}
