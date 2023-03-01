using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.TestCommon.DbCleanup
{
    public class CleanDatabase : BaseRepository, ICleanDatabase
    {
        public CleanDatabase(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task Execute()
        {
            var parameters = new DynamicParameters();

            parameters.Add("@includeUserTable", true, DbType.Boolean);

            await WithConnection(async c => await c.ExecuteAsync(
                "[employer_account].[Cleardown]",
                parameters,
                commandType: CommandType.StoredProcedure));
        }
    }
}