using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using Moq;
using SFA.DAS.Configuration;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers
{
    public class FinanceStatisticsDataHelper
    {
        private const string ServiceName = "SFA.DAS.LevyAggregationProvider";
        private readonly LevyDeclarationProviderConfiguration _configuration;

        public FinanceStatisticsDataHelper()
        {
            _configuration = ConfigurationHelper.GetConfiguration<LevyDeclarationProviderConfiguration>(ServiceName);
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            using (var connection = new SqlConnection(_configuration.DatabaseConnectionString))
            {
                return await connection.QuerySingleAsync<StatisticsViewModel>(Sql);
            }
        }

        private const string Sql = @"
select count(0) as TotalPayments
from employer_financial.Payment;";

        public async Task CreateFinanceStatistics()
        {
            var fixture = new Fixture();

            var financeDbContext = new EmployerFinanceDbContext(_configuration.DatabaseConnectionString);
            var lazyDb = new Lazy<EmployerFinanceDbContext>(() => financeDbContext);
            var levyRepository = new DasLevyRepository(_configuration, Mock.Of<ILog>(), lazyDb);

            financeDbContext.Database.BeginTransaction();

            try
            {
                await levyRepository.CreatePayments(new List<PaymentDetails>
                {
                    fixture
                        .Build<PaymentDetails>()
                        .With(details => details.CollectionPeriodId, "R05")
                        // could put sanitised collection period and delivery period values in for mth and year
                        .With(details => details.PeriodEnd, "R12")
                        .With(details => details.EmployerAccountVersion, $"ver-{DateTime.Now.Ticks.ToString().Substring(4,10)}")
                        .With(details => details.ApprenticeshipVersion, $"ver-{DateTime.Now.Ticks.ToString().Substring(4,10)}")
                        .Without(details => details.FrameworkCode)
                        .Without(details => details.PathwayCode)
                        .Without(details => details.PathwayName)
                        .Create()
                });

                financeDbContext.Database.CurrentTransaction.Commit();
            }
            catch (Exception e)
            {
                financeDbContext.Database.CurrentTransaction.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }
    }
}