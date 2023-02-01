using MediatR;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.SplitExpense.Commands.UpdateSplitExpense;

public sealed record UpdateSplitExpenseCommand(
    Guid UserId,
    IReadOnlyCollection<Guid> UserIds,
    Guid ExpenseId) : IRequest<Result>;
