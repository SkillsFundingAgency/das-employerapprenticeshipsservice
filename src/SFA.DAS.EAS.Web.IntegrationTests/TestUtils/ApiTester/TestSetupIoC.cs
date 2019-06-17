using System;
using System.Data.Common;
using System.Data.SqlClient;
using Moq;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    /// <summary>
    ///     IoC setup for creating test data. 
    /// </summary>
    static class TestSetupIoC
    {
        public static IContainer CreateIoC()
        {
            var ioc = new Container(config =>
            {

                config.For<ILog>().Use(new Mock<ILog>().Object).Singleton();

                SetUpAccountIoC(config);
                SetUpFinanceIoC(config);
            });

            return ioc;
        }

        private static void SetUpAccountIoC(ConfigurationExpression config)
        {
            config.AddRegistry<ConfigurationRegistry>();
            config.AddRegistry<HashingRegistry>();
            config.AddRegistry<RepositoriesRegistry>();
            config.AddRegistry<AutoConfigurationRegistry>();

            //this needs to be conditional based on environment...? or just read from the azure local storage - using something maybe test helper?
            //var accountConfiguration = new EmployerApprenticeshipsServiceConfiguration()
            //{
            //    DatabaseConnectionString = System.Configuration.ConfigurationManager.AppSettings["AccountsDatabaseConnectionString"]
            //};
            //let's just wire up using auto config and get the instance that way as per the real code

            config.For<DbConnection>().Use(context => new SqlConnection(context.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            //config.For<EmployerApprenticeshipsServiceConfiguration>().Use(c => c.GetInstance<>() accountConfiguration);
            config.For<EmployerAccountsDbContext>().Use(context => new EmployerAccountsDbContext(context.GetInstance<EmployerApprenticeshipsServiceConfiguration>().DatabaseConnectionString));
            config.For<Lazy<EmployerAccountsDbContext>>().Use(context => new Lazy<EmployerAccountsDbContext>(() => context.GetInstance<EmployerAccountsDbContext>()));
        }

        private static void SetUpFinanceIoC(ConfigurationExpression config)
        {
            var financeConfiguration = new LevyDeclarationProviderConfiguration()
            {
                DatabaseConnectionString = System.Configuration.ConfigurationManager.AppSettings["FinanceDatabaseConnectionString"]
            };

            config.For<LevyDeclarationProviderConfiguration>().Use(financeConfiguration);
            config.For<EmployerFinanceDbContext>().Use(context => new EmployerFinanceDbContext(financeConfiguration.DatabaseConnectionString));
        }
    }
}