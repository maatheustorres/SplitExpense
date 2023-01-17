using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;

namespace SplitExpense.Persistence.Repositories;

internal sealed class GroupRepository : GenericRepository<Group>, IGroupRepository
{
    public GroupRepository(IDbContext dbContext) 
        : base(dbContext) { }
}
