using SplitExpense.Domain.Entities;

namespace SplitExpense.Domain.Repositories;

public interface IExpenseUserRepository
{
    Task<bool> CheckIfAddedToExpense(ExpenseUsers expenseUser);
    void InsertRange(IReadOnlyCollection<ExpenseUsers> expenseUsers);
    Task<ExpenseUsers> GetByIdAsync(Guid expenseId);
    void Update(ExpenseUsers expenseUser);
    Task<ExpenseUsers> GetByUserIdAndExpenseId(Guid id1, Guid id2);
}
