using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferRepository : BaseRepository, ITransferRepository
    {
        private readonly float _allowancePercentage;

        public TransferRepository(LevyDeclarationProviderConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _allowancePercentage = configuration.TransferAllowancePercentage;
        }

        public async Task<decimal> GetTransferAllowance(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@allowancePercentage", _allowancePercentage, DbType.Single);

                return await c.QuerySingleOrDefaultAsync<decimal?>(
                    sql: "[employer_financial].[GetAccountTransferAllowance]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result ?? 0;
        }

        public Task CreateAccountTransfers(IEnumerable<AccountTransfer> transfers)
        {
            throw new System.NotImplementedException();
        }
    }
}
