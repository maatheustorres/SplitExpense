using MediatR;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Expenses.Commands.SplitExpense;

public sealed record SplitExpenseCommand(
    Guid GroupId,
    Guid UserId,
    IReadOnlyCollection<Guid> UserIds,
    Guid ExpenseId) : IRequest<Result>;
