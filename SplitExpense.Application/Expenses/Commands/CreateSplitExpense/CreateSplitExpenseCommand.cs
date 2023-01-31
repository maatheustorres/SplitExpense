using MediatR;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Expenses.Commands.CreateSplitExpense;

public sealed record CreateSplitExpenseCommand(
    Guid UserId,
    IReadOnlyCollection<Guid> UserIds,
    Guid ExpenseId) : IRequest<Result>;
