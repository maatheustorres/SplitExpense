using SplitExpense.Domain.Entities;

namespace SplitExpense.Domain.Repositories;

public interface IUserGroupRepository
{
    void InsertRange(IReadOnlyCollection<UserGroup> usersGroup);
    Task<bool> CheckIfAddedToGroup(UserGroup userGroup);
}
