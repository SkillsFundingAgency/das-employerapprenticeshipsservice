using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferRepository : BaseRepository, ITransferRepository
    {
        private readonly float _allowanceRatio;

        public TransferRepository(LevyDeclarationProviderConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _allowanceRatio = configuration.TransferAllowanceRatio;
        }

        public async Task<decimal> GetTransferAllowance(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@allowanceRatio", _allowanceRatio, DbType.Single);

                return await c.QuerySingleOrDefaultAsync<decimal?>(
                    sql: "[employer_financial].[GetAccountTransferAllowance]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result ?? 0;
        }
    }
}
