using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.TestCommon.DbCleanup
{
    public class UpdateTransactionLine : BaseRepository, IUpdateTransactionLine
    {
        public UpdateTransactionLine(LevyDeclarationProviderConfiguration configuration, ILog logger) : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task Execute(long submissionId, DateTime createdDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@submissionId", submissionId, DbType.Int64);
            parameters.Add("@createdDate", createdDate, DbType.DateTime);

            await WithConnection(async c => await c.ExecuteAsync(
                "[employer_financial].[UpdateTransactionLineDate_BySubmissionId]",
                parameters,
                commandType: CommandType.StoredProcedure));
        }
    }
}