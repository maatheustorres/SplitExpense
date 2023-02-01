using MediatR;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Expense;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using static SplitExpense.Contracts.Expense.ExpensesResponse;

namespace SplitExpense.Application.Expenses.Queries.GetExpensesByGroupId;

public sealed class GetExpensesByGroupIdQueryHandler : IRequestHandler<GetExpensesByGroupIdQuery, ResultT<List<ExpensesResponse>>>
{
    private readonly IDbContext _dbContext;
    private readonly IExpenseUserRepository _expenseUserRepository;

    public GetExpensesByGroupIdQueryHandler(IDbContext dbContext, IExpenseUserRepository expenseUserRepository)
    {
        _dbContext = dbContext;
        _expenseUserRepository = expenseUserRepository;
    }

    public async Task<ResultT<List<ExpensesResponse>>> Handle(
        GetExpensesByGroupIdQuery request,
        CancellationToken cancellationToken)
    {
        List<ExpensesResponse> expenses = await (
            from expense in _dbContext.Set<Expense>().AsNoTracking()
            join usergroup in _dbContext.Set<UserGroup>().AsNoTracking()
                on expense.UserGroupId equals usergroup.Id
            join user in _dbContext.Set<User>().AsNoTracking()
                on usergroup.UserId equals user.Id
            where usergroup.GroupId == request.GroupId
            select new ExpensesResponse
            {
                UserGroupId = usergroup.Id,
                UserId = user.Id,
                UserToReceive = $"{user.FirstName} {user.LastName}",
                TotalExpense = expense.TotalExpense,
                Paid = expense.Paid,
                ExpenseId = expense.Id
            }).ToListAsync();

        if (!expenses.Any())
        {
            return Result.Failure<List<ExpensesResponse>>(DomainErrors.Expense.NotFound);
        }

        var expenseIds = expenses
            .Where(x => x.ExpenseId != null)
            .Select(expense => expense.ExpenseId)
            .Distinct()
            .ToList();

        foreach (var expenseId in expenseIds)
        {
            List<UserExpenseListResponseModel> userExpenseList = await (
                from expenseUser in _dbContext.Set<ExpenseUsers>().AsNoTracking()
                join user in _dbContext.Set<User>().AsNoTracking()
                    on expenseUser.UserId equals user.Id
                where expenseUser.ExpenseId == expenseId
                select new UserExpenseListResponseModel
                {
                    Id = expenseUser.Id,
                    Repay = expenseUser.PayTo,
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}"
                }).ToListAsync();

            var expense = expenses.FirstOrDefault(x => x.ExpenseId == expenseId);

            if (expense is not null && userExpenseList.Count > 0)
                expense.UserExpense.AddRange(userExpenseList);
        }

        return Result.Success(expenses);
    }
}
