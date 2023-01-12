namespace SplitExpense.Application.Core.Abstractions.Common;

public interface IDateTime
{
    DateTime UtcNow { get; }
}
