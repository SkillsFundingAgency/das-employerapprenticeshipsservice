using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Extensions;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class DatabaseServiceRegistrations
{
    public static IServiceCollection AddDatabaseRegistration(this IServiceCollection services, string databaseConnectionString)
    {
        var dbConnection = DatabaseExtensions.GetSqlConnection(databaseConnectionString);
        
        services.AddDbContext<EmployerAccountsDbContext>(options => options.UseSqlServer(dbConnection), ServiceLifetime.Transient);

        services.AddTransient(provider => new Lazy<EmployerAccountsDbContext>(provider.GetService<EmployerAccountsDbContext>()));

        return services;
    }
}