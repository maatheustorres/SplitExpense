using Microsoft.EntityFrameworkCore;
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
    public async Task<User> GetByEmailAsync(Email email) => await FirstOrDefaultAsync(new UserWithEmailSpecification(email));

    public async Task<IReadOnlyCollection<User>> GetUsersByEmailsAsync(IReadOnlyCollection<Email> emails) =>
        emails.Any()
            ? await DbContext.Set<User>().Where(new UsersGroupWithEmailSpecification(emails)).ToArrayAsync()
            : Array.Empty<User>();
}
