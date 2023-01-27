using MediatR;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Expenses.Commands.AddUserToExpense;

public sealed record AddUserToExpenseCommand(
    Guid GroupId,
    IReadOnlyCollection<Guid> UserIds,
    Guid ExpenseId,
    decimal Pay) : IRequest<Result>;
