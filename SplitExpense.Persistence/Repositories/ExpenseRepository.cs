using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;

namespace SplitExpense.Persistence.Repositories;

internal sealed class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
{
    public ExpenseRepository(IDbContext dbContext) 
        : base(dbContext) { }
}
