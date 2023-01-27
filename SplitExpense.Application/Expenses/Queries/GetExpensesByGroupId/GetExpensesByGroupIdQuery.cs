using MediatR;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Expenses.Queries.GetExpensesByGroupId;

public sealed record GetExpensesByGroupIdQuery(Guid GroupId) : IRequest<ResultT<List<ExpensesResponse>>>;
