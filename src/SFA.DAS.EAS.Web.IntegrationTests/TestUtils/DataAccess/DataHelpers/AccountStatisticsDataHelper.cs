using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers
{
    internal class AccountStatisticsDataHelper
    {
        private readonly string _databaseConnectionString;
        
        public AccountStatisticsDataHelper(string databaseConnectionString)
        {
            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            using (var connection = new SqlConnection(_databaseConnectionString))
            {   
                return await connection.QuerySingleAsync<StatisticsViewModel>(GetStatisticsSql);
            }
        }

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

        /*public async Task<int> AddAccount(CreateAccountModel model)
        {
            using (var connection = new SqlConnection(_databaseConnectionString))
            {
                return await connection.ExecuteAsync("[employer_account].[CreateAccount]", model, 
                    connection., null, CommandType.StoredProcedure);
            }
        }*/
    }

    internal class CreateAccountModel
    {
        public long UserId { get; set; }
        public string EmployerNumber { get; set; }
    }
}