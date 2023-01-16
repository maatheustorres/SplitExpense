using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SplitExpense.Application.Core.Abstractions.Common;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Domain.Core.Primitives;
using System.Reflection;

namespace SplitExpense.Persistence;

public sealed class SplitExpenseDbContext : DbContext, IDbContext, IUnitOfWork
{
    private readonly IDateTime _dateTime;
    private readonly IMediator _mediator;

    public SplitExpenseDbContext(DbContextOptions options, IDateTime dateTime, IMediator mediator)
        : base(options)
    {
        _dateTime = dateTime;
        _mediator = mediator;
    }

    public new DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity
        => base.Set<TEntity>();

    public async Task<TEntity> GetByIdAsync<TEntity>(Guid id)
        where TEntity : Entity
        => id == Guid.Empty ?
            null :
            await Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);

    public void Insert<TEntity>(TEntity entity)
        where TEntity : Entity
        => Set<TEntity>().Add(entity);

    public void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities) 
        where TEntity : Entity
        => Set<TEntity>().AddRange(entities);

    public void Remove<TEntity>(TEntity entity) 
        where TEntity : Entity
        => Set<TEntity>().Remove(entity);

    public Task<int> ExecuteSqlAsync(string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => Database.BeginTransactionAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
