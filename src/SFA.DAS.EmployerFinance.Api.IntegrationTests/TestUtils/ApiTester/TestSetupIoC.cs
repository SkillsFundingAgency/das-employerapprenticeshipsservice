using System;
using System.Data.Common;
using System.Data.SqlClient;
using Moq;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.DependencyResolution;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.ApiTester
{
    /// <summary>
    ///     IoC setup for creating test data. 
    /// </summary>
    static class TestSetupIoC
    {
        private static readonly Lazy<EmployerAccountsConfiguration> LazyAccountConfiguration = 
            new Lazy<EmployerAccountsConfiguration>(GetAccountConfiguration);

        private static readonly Lazy<EmployerFinanceConfiguration> LazyFinanceConfiguration = 
            new Lazy<EmployerFinanceConfiguration>(GetFinanceConfiguration);

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

            var accountConfiguration = LazyAccountConfiguration.Value;
            var dbContext = new EmployerAccountsDbContext(accountConfiguration.DatabaseConnectionString);

            config.For<DbConnection>().Use(context => new SqlConnection(accountConfiguration.DatabaseConnectionString));
            config.For<EmployerAccountsConfiguration>().Use(accountConfiguration);
            config.For<EmployerAccountsDbContext>().Use(context => dbContext);
            config.For<Lazy<EmployerAccountsDbContext>>().Use(context => new Lazy<EmployerAccountsDbContext>(() => dbContext));
        }

        private static void SetUpFinanceIoC(ConfigurationExpression config)
        {
            var financeConfiguration = LazyFinanceConfiguration.Value;
            config.For<EmployerFinanceConfiguration>().Use(financeConfiguration);
            config.For<EmployerFinanceDbContext>().Use(context => new EmployerFinanceDbContext(financeConfiguration.DatabaseConnectionString));
        }

        private static EmployerAccountsConfiguration GetAccountConfiguration()
        {
            return ConfigurationHelper.GetConfiguration<EmployerAccountsConfiguration>(Constants.ServiceName);
        }

        private static EmployerFinanceConfiguration GetFinanceConfiguration()
        {
            return ConfigurationHelper.GetConfiguration<EmployerFinanceConfiguration>("SFA.DAS.LevyAggregationProvider");
        }
    }
}