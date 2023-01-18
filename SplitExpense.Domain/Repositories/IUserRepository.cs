using SplitExpense.Domain.Entities;
using SplitExpense.Domain.ValueObjects;

namespace SplitExpense.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> IsEmailUniqueAsync(Email email);
    void Insert(User user);
    Task<User> GetByEmailAsync(Email email);
    Task<User> GetByIdAsync(Guid userId);
    Task<IReadOnlyCollection<User>> GetUsersByEmailsAsync(IReadOnlyCollection<Email> emails);
}
