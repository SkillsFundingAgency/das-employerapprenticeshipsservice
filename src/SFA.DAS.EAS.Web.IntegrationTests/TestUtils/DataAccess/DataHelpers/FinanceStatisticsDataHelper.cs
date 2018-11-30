using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers
{
    public class FinanceStatisticsDataHelper
    {
        private readonly string _databaseConnectionString;

        public FinanceStatisticsDataHelper(string databaseConnectionString)
        {
            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            using (var connection = new SqlConnection(_databaseConnectionString))
            {
                
                return await connection.QuerySingleAsync<StatisticsViewModel>(Sql);
            }
        }

        private const string Sql = @"
select count(0) as TotalPayments
from employer_financial.Payment;";
    }
}