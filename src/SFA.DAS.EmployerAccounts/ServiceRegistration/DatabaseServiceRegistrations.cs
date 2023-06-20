using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class DatabaseServiceRegistrations
{
    public static IServiceCollection AddDatabaseRegistration(this IServiceCollection services)
    {
        services.AddDbContext<EmployerAccountsDbContext>((sp, options) =>
        {
            var dbConnection = DatabaseExtensions.GetSqlConnection(sp.GetService<EmployerAccountsConfiguration>().DatabaseConnectionString);
            options.UseSqlServer(dbConnection);
        }, ServiceLifetime.Transient);

        services.AddTransient(provider => new Lazy<EmployerAccountsDbContext>(provider.GetService<EmployerAccountsDbContext>()));

        return services;
    }
}