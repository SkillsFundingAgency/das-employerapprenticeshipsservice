using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Configuration;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers
{
    public class FinanceStatisticsDataHelper
    {
        private const string ServiceName = "SFA.DAS.LevyAggregationProvider";
        private const string GetStatisticsSql = @"select count(0) as TotalPayments from employer_financial.Payment;";

        private readonly LevyDeclarationProviderConfiguration _configuration;

        public FinanceStatisticsDataHelper()
        {
            _configuration = ConfigurationHelper.GetConfiguration<LevyDeclarationProviderConfiguration>(ServiceName);
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            using (var connection = new SqlConnection(_configuration.DatabaseConnectionString))
            {
                return await connection.QuerySingleAsync<StatisticsViewModel>(GetStatisticsSql);
            }
        }

        public async Task CreateFinanceStatistics()
        {
            var dbBuilderRuntime = new DbBuilderRuntime();
            await dbBuilderRuntime.RunDbBuilder<EmployerFinanceDbBuilder>(async builder =>
            {
                var data = new TestModelBuilder()
                    .WithNewPayment();
                await builder.SetupDataAsync(data);
            });
        }
    }
}