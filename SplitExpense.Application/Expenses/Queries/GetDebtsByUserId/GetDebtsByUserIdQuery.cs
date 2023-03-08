using MediatR;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Expenses.Queries.GetDebtsByUserId;

public sealed record GetDebtsByUserIdQuery(Guid UserId) : IRequest<ResultT<List<DebtsResponse>>>;
