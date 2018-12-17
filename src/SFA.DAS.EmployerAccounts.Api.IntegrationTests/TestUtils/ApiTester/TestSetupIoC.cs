using System;
using System.Data.Common;
using System.Data.SqlClient;
using Moq;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester
{
    static class TestSetupIoC
    {
        private static readonly Lazy<EmployerAccountsConfiguration> LazyAccountConfiguration = 
            new Lazy<EmployerAccountsConfiguration>(GetAccountConfiguration);

        //private static readonly Lazy<LevyDeclarationProviderConfiguration> LazyFinanceConfiguration = 
        //    new Lazy<LevyDeclarationProviderConfiguration>(GetFinanceConfiguration);

        public static IContainer CreateIoC()
        {
            var ioc = new Container(config =>
            {

                config.For<ILog>().Use(new Mock<ILog>().Object).Singleton();

                SetUpAccountIoC(config);
                //SetUpFinanceIoC(config);
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

        //private static void SetUpFinanceIoC(ConfigurationExpression config)
        //{
        //    var financeConfiguration = LazyFinanceConfiguration.Value;
        //    config.For<LevyDeclarationProviderConfiguration>().Use(financeConfiguration);
        //    config.For<EmployerFinanceDbContext>().Use(context => new EmployerFinanceDbContext(financeConfiguration.DatabaseConnectionString));
        //}

        private static EmployerAccountsConfiguration GetAccountConfiguration()
        {
            return ConfigurationHelper.GetConfiguration<EmployerAccountsConfiguration>(Constants.ServiceName);
        }

        //private static LevyDeclarationProviderConfiguration GetFinanceConfiguration()
        //{
        //    return ConfigurationHelper.GetConfiguration<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider");
        //}
    }
}