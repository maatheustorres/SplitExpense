using SplitExpense.Domain.Entities;
using System.Linq.Expressions;

namespace SplitExpense.Persistence.Specifications;

internal sealed class ExpenseUsersSpecification : Specification<ExpenseUsers>
{
    private readonly Guid _userId;
    private readonly Guid _expenseId;

    public ExpenseUsersSpecification(ExpenseUsers expenseUsers)
    {
        _userId = expenseUsers.UserId;
        _expenseId = expenseUsers.ExpenseId;
    }

    internal override Expression<Func<ExpenseUsers, bool>> ToExpression() =>
        expenseUsers => expenseUsers.UserId == _userId && expenseUsers.ExpenseId == _expenseId;
}
