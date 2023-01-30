using MediatR;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Expenses.Commands.UpdateExpense;

public sealed record UpdateExpenseCommand(
    Guid Id,
    decimal Expense,
    bool Paid,
    Guid UserId,
    Guid UserGroupId) : IRequest<ResultT<ExpensesResponse>>;

