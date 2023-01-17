using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Entities;
using SplitExpense.Domain.Repositories;
using SplitExpense.Domain.ValueObjects;
using SplitExpense.Persistence.Specifications;

namespace SplitExpense.Persistence.Repositories;

internal sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IDbContext dbContext) 
        : base(dbContext) { }

    public async Task<bool> IsEmailUniqueAsync(Email email) => !await AnyAsync(new UserWithEmailSpecification(email));
}
