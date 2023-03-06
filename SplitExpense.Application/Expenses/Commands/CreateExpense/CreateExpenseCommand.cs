using MediatR;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Primitives.Result;

namespace SplitExpense.Application.Expenses.Commands.CreateExpense;

public sealed record CreateExpenseCommand(
    decimal TotalExpense, 
    bool Paid, 
    Guid UserGroupId,
    Guid UserId,
    Guid GroupId) : IRequest<ResultT<ExpenseResponse>>;
    
