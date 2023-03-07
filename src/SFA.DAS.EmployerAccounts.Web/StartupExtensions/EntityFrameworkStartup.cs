﻿using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NServiceBus.Persistence;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.NServiceBus.SqlServer.Data;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions
{
    public static class EntityFrameworkStartup
    {
        public static IServiceCollection AddEntityFramework(this IServiceCollection services, EmployerAccountsConfiguration config)
        {
            return services.AddScoped(p =>
            {
                var unitOfWorkContext = p.GetService<IUnitOfWorkContext>();
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                EmployerAccountsDbContext dbContext;
                try
                {                    
                    var synchronizedStorageSession = unitOfWorkContext.Get<SynchronizedStorageSession>();
                    var sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();
                    var optionsBuilder = new DbContextOptionsBuilder<EmployerAccountsDbContext>().UseSqlServer(sqlStorageSession.Connection);                    
                    dbContext = new EmployerAccountsDbContext(sqlStorageSession.Connection, config, optionsBuilder.Options, azureServiceTokenProvider);
                    dbContext.Database.UseTransaction(sqlStorageSession.Transaction);
                }
                catch (KeyNotFoundException)
                {
                    var connection = DatabaseExtensions.GetSqlConnection(config.DatabaseConnectionString);
                    var optionsBuilder = new DbContextOptionsBuilder<EmployerAccountsDbContext>().UseSqlServer(connection);
                    dbContext = new EmployerAccountsDbContext(optionsBuilder.Options);
                }

                return dbContext;
            });
        }
    }
}