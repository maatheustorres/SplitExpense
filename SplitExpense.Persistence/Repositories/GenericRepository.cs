using Microsoft.EntityFrameworkCore;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Persistence.Specifications;

namespace SplitExpense.Persistence.Repositories;

internal abstract class GenericRepository<TEntity>
    where TEntity : Entity
{
    protected GenericRepository(IDbContext dbContext) => DbContext = dbContext;

    public IDbContext DbContext { get; }

    public async Task<TEntity> GetByIdAsync(Guid id) => await DbContext.GetByIdAsync<TEntity>(id);

    public void Insert(TEntity entity) => DbContext.Insert(entity);

    public void InsertRange(IReadOnlyCollection<TEntity> entities) => DbContext.InsertRange(entities);

    public void Update(TEntity entity) => DbContext.Set<TEntity>().Update(entity);

    public void Remove(TEntity entity) => DbContext.Remove(entity);

    protected async Task<bool> AnyAsync(Specification<TEntity> specification) =>
        await DbContext.Set<TEntity>().AnyAsync(specification);

    protected async Task<TEntity> FirstOrDefaultAsync(Specification<TEntity> specification) =>
        await DbContext.Set<TEntity>().FirstOrDefaultAsync(specification);
}