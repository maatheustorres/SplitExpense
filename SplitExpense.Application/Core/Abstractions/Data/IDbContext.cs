using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Domain.Core.Primitives;

namespace SplitExpense.Application.Core.Abstractions.Data;

public interface IDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity;

    Task<TEntity> GetByIdAsync<TEntity>(Guid id)
        where TEntity : Entity;

    void Insert<TEntity>(TEntity entity)
        where TEntity : Entity;

    void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity;

    void Remove<TEntity>(TEntity entity)
        where TEntity : Entity;

}
