using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Configuration;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers
{
    internal class AccountStatisticsDataHelper
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private const string GetStatisticsSql = @"
select (
  select count(0)
  from employer_account.Account
) as TotalAccounts, (
  select count(0)
  from employer_account.LegalEntity
) as TotalLegalEntities, (
  select count(0)
  from employer_account.Paye
) as TotalPayeSchemes, (
  select count(0)
  from employer_account.EmployerAgreement a
  where a.StatusId = 2 -- signed
) as TotalAgreements;";

        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        
        public AccountStatisticsDataHelper()
        {
            _configuration = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(ServiceName);
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            using (var connection = new SqlConnection(_configuration.DatabaseConnectionString))
            {   
                return await connection.QuerySingleAsync<StatisticsViewModel>(GetStatisticsSql);
            }
        }

        public async Task CreateAccountStatistics()
        {
            var dbBuilderRuntime = new DbBuilderRuntime();
            await dbBuilderRuntime.RunDbBuilder<EmployerAccountsDbBuilder>(async builder =>
            {
                var data = new TestModelBuilder()
                    .WithNewUser()
                    .WithNewAccount();
                await builder.SetupDataAsync(data);
            });
        }
    }
}