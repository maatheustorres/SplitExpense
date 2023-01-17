using SplitExpense.Domain.Entities;

namespace SplitExpense.Domain.Repositories;

public interface IGroupRepository
{
    void Insert(Group group);
}
