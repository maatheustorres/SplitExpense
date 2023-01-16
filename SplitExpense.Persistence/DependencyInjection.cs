using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Persistence.Infrastructure;

namespace SplitExpense.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionString.SettingsKey);

        services.AddSingleton(new ConnectionString(connectionString));

        services.AddDbContext<SplitExpenseDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<SplitExpenseDbContext>());

        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<SplitExpenseDbContext>()); 

        return services;
    }
}
