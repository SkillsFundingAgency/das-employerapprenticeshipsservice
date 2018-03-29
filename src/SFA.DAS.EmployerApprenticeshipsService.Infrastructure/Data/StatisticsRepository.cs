using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Entities;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class StatisticsRepository : BaseRepository, IStatisticsRepository
    {
        public async Task<Statistics> GetStatistics()
        {
            var result = await WithConnection(async c => await c.QuerySingleOrDefaultAsync<Statistics>(
                sql: "[employer_account].[GetStatistics]",
                commandType: CommandType.StoredProcedure));

            return result;
        }

        public StatisticsRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }
    }
}
