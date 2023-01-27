using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Persistence.Specifications;

namespace SplitExpense.Persistence.Repositories;

internal class UserGroupRepository : GenericRepository<UserGroup>, IUserGroupRepository
{
    public UserGroupRepository(IDbContext dbContext) 
        : base(dbContext) { }

    public async Task<bool> CheckIfAddedToGroup(UserGroup userGroup) => await AnyAsync(new UsersGroupSpecification(userGroup));
}
