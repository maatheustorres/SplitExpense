using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Domain.Repositories;

public interface IUserGroupRepository
{
    void InsertRange(IReadOnlyCollection<UserGroup> usersGroup);
    Task<bool> CheckIfAddedToGroup(UserGroup userGroup);
    Task<UserGroup> GetByIdAsync(Guid userId);
}
