using MediatR;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Expenses.Queries.GetExpensesToBeReceivedByUserId;

public sealed record GetExpensesToBeReceivedByUserIdQuery(Guid UserId) : IRequest<ResultT<List<ExpenseToBeReceivedResponse>>>;
