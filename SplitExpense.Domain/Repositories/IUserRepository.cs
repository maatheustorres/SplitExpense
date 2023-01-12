using SplitExpense.Domain.Entities;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> IsEmailUniqueAsync(Email value);
    void Insert(User user);
}
