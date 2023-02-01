using MediatR;
using SplitExpense.Contracts.SplitExpense;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.SplitExpense.Commands.CreateSplitExpense;

public sealed record CreateSplitExpenseCommand(
    Guid UserId,
    IReadOnlyCollection<Guid> UserIds,
    Guid ExpenseId) : IRequest<Result>;
