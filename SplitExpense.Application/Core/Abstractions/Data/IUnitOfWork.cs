using Microsoft.EntityFrameworkCore.Storage;

namespace SplitExpense.Application.Core.Abstractions.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
