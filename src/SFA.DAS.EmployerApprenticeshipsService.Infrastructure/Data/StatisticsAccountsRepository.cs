using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Entities.Statistics;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class StatisticsAccountsRepository : BaseRepository, IStatisticsAccountsRepository
    {
        public async Task<StatisticsAccounts> GetStatistics()
        {
            var result = await WithConnection(async c => await c.QuerySingleOrDefaultAsync<StatisticsAccounts>(
                sql: "[employer_account].[GetStatistics]",
                commandType: CommandType.StoredProcedure));

            return result;
        }

        public StatisticsAccountsRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }
    }
}
