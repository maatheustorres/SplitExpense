using Microsoft.EntityFrameworkCore;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Persistence.Specifications;

namespace SplitExpense.Persistence.Repositories;

internal sealed class ExpenseUserRepository : GenericRepository<ExpenseUsers>, IExpenseUserRepository
{
    public ExpenseUserRepository(IDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<bool> CheckIfAddedToExpense(ExpenseUsers expenseUser) => await AnyAsync(new ExpenseUsersSpecification(expenseUser));

    public async Task<ExpenseUsers> GetByUserIdAndExpenseId(Guid expenseId, Guid userId)
        => await DbContext.Set<ExpenseUsers>()
                .FirstOrDefaultAsync(x => x.ExpenseId == expenseId && x.UserId == userId);
}
