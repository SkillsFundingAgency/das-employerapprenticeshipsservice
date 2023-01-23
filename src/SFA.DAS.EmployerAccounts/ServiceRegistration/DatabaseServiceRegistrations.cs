using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class DatabaseServiceRegistrations
{
    public static IServiceCollection AddDatabaseRegistration(this IServiceCollection services, EmployerAccountsConfiguration config, string environmentName)
    {
        if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<EmployerAccountsDbContext>(options => options.UseSqlServer(config.DatabaseConnectionString), ServiceLifetime.Transient);
        }
        else
        {
            services.AddDbContext<EmployerAccountsDbContext>(ServiceLifetime.Transient);
        }

        services.AddTransient<EmployerAccountsDbContext, EmployerAccountsDbContext>(provider => provider.GetService<EmployerAccountsDbContext>());
        services.AddTransient(provider => new Lazy<EmployerAccountsDbContext>(provider.GetService<EmployerAccountsDbContext>()));

        return services;
    }
}