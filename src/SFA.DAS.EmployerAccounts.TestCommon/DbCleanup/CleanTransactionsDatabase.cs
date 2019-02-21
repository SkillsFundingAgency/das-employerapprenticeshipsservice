using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.TestCommon.DbCleanup
{
    public class CleanTransactionsDatabase : BaseRepository, ICleanTransactionsDatabase
    {
        public CleanTransactionsDatabase(LevyDeclarationProviderConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task Execute()
        {

            var parameters = new DynamicParameters();
            parameters.Add("@INCLUDETOPUPTABLE", 1, DbType.Int16);
            await WithConnection(async c => await c.ExecuteAsync(
                "[employer_financial].[Cleardown]",
                parameters,
                commandType: CommandType.StoredProcedure));
            
        }
    }
}
