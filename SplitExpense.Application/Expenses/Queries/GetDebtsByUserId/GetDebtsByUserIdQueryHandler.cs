using MediatR;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Application.Expenses.Queries.GetDebtsByUserId;

public sealed class GetDebtsByUserIdQueryHandler : IRequestHandler<GetDebtsByUserIdQuery, ResultT<List<DebtsResponse>>>
{
    private readonly IDbContext _dbContext;

    public GetDebtsByUserIdQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResultT<List<DebtsResponse>>> Handle(GetDebtsByUserIdQuery request, CancellationToken cancellationToken)
    {
        List<DebtsResponse> debts = await (
            from expenseUsers in _dbContext.Set<ExpenseUsers>().AsNoTracking()
            join expense in _dbContext.Set<Expense>().AsNoTracking()
                on expenseUsers.ExpenseId equals expense.Id
            join userGroup in _dbContext.Set<UserGroup>().AsNoTracking()
                on expense.UserGroupId equals userGroup.Id
            join groupName in _dbContext.Set<Group>().AsNoTracking()
                on userGroup.GroupId equals groupName.Id
            join users in _dbContext.Set<User>().AsNoTracking()
                on userGroup.UserId equals users.Id
            where expenseUsers.UserId == request.UserId && expense.Paid != true
            select new DebtsResponse
            {
                PayTo = expenseUsers.PayTo,
                TotalExpense = expense.TotalExpense,
                GroupName = groupName.Name,
                UserToReceive = $"{users.FirstName} {users.LastName}"
            }).ToListAsync();

        if (!debts.Any())
        {
            return Result.Failure<List<DebtsResponse>>(DomainErrors.Debts.NotFound);
        }

        return Result.Success(debts);
    }
}
