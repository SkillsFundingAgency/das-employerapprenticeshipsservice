using System;
using System.Data.Common;
using System.Data.SqlClient;
using Moq;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Testing.Helpers;
using StructureMap;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    /// <summary>
    ///     IoC setup for creating test data. 
    /// </summary>
    static class TestSetupIoC
    {
        private static readonly Lazy<EmployerApprenticeshipsServiceConfiguration> LazyAccountConfiguration =
            new Lazy<EmployerApprenticeshipsServiceConfiguration>(GetAccountConfiguration);

        private static readonly Lazy<LevyDeclarationProviderConfiguration> LazyFinanceConfiguration =
            new Lazy<LevyDeclarationProviderConfiguration>(GetFinanceConfiguration);

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

            var accountConfiguration = LazyAccountConfiguration.Value;
            var dbContext = new EmployerAccountsDbContext(accountConfiguration.DatabaseConnectionString);

            config.For<DbConnection>().Use(context => new SqlConnection(accountConfiguration.DatabaseConnectionString));
            config.For<EmployerApprenticeshipsServiceConfiguration>().Use(accountConfiguration);
            config.For<EmployerAccountsDbContext>().Use(context => dbContext);
            config.For<Lazy<EmployerAccountsDbContext>>().Use(context => new Lazy<EmployerAccountsDbContext>(() => dbContext));
        }

        private static void SetUpFinanceIoC(ConfigurationExpression config)
        {
            var financeConfiguration = LazyFinanceConfiguration.Value;
            config.For<LevyDeclarationProviderConfiguration>().Use(financeConfiguration);
            config.For<EmployerFinanceDbContext>().Use(context => new EmployerFinanceDbContext(financeConfiguration.DatabaseConnectionString));
        }

        private static EmployerApprenticeshipsServiceConfiguration GetAccountConfiguration()
        {
            return ConfigurationTestHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(Domain.Constants.ServiceName);
        }

        private static LevyDeclarationProviderConfiguration GetFinanceConfiguration()
        {
            return ConfigurationTestHelper.GetConfiguration<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider");
        }
    }
}