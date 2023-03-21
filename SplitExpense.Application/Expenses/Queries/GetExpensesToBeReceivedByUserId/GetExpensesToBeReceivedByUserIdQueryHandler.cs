using MediatR;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Application.Expenses.Queries.GetExpensesToBeReceivedByUserId;

public sealed class GetExpensesToBeReceivedByUserIdQueryHandler : IRequestHandler<GetExpensesToBeReceivedByUserIdQuery, ResultT<List<ExpenseToBeReceivedResponse>>>
{
    private readonly IDbContext _dbContext;

    public GetExpensesToBeReceivedByUserIdQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResultT<List<ExpenseToBeReceivedResponse>>> Handle(GetExpensesToBeReceivedByUserIdQuery request, CancellationToken cancellationToken)
    {
        List<ExpenseToBeReceivedResponse> expenseToBeReceived = await (
            from expense in _dbContext.Set<Expense>().AsNoTracking()
            join userGroup in _dbContext.Set<UserGroup>().AsNoTracking()
                on expense.UserGroupId equals userGroup.Id
            join users in _dbContext.Set<User>().AsNoTracking()
                on userGroup.UserId equals users.Id
            join groupName in _dbContext.Set<Group>().AsNoTracking()
                on userGroup.GroupId equals groupName.Id
            where users.Id == request.UserId && expense.Paid == false
            select new ExpenseToBeReceivedResponse
            {
                GroupId = userGroup.GroupId,
                UserGroupId = userGroup.Id,
                GroupName = groupName.Name,
                TotalExpense = expense.TotalExpense,
                Username = $"{users.FirstName} {users.LastName}"
            }).ToListAsync();

        if (expenseToBeReceived.Any())
            return Result.Success(expenseToBeReceived);

        return Result.Failure<List<ExpenseToBeReceivedResponse>>(DomainErrors.Expense.NoExpenseToBeReceived);
    }
}
