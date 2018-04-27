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
    public class StatisticsFinancialRepository : BaseRepository, IStatisticsFinancialRepository
    {
        public StatisticsFinancialRepository(LevyDeclarationProviderConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<StatisticsFinancial> GetStatistics()
        {
            var result = await WithConnection(async c => await c.QuerySingleOrDefaultAsync<StatisticsFinancial>(
                sql: "[employer_financial].[GetStatistics]",
                commandType: CommandType.StoredProcedure));

            return result;
        }
    }
}
