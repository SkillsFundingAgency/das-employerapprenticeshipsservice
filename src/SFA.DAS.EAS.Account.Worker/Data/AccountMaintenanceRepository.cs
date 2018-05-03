using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Account.Worker.Extensions;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EAS.Account.Worker.Data
{
    public class AccountMaintenanceRepository : BaseRepository, IAccountMaintenanceRepository
    {
        public AccountMaintenanceRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
        }

        public async Task<List<long>> GetAccountsMissingPublicHashedId()
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                return await c.QueryAsync<long>(
                    sql: "[employer_account].[GetAccountsMissingPublicHashedId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task UpdateAccountPublicHashedIds(IEnumerable<KeyValuePair<long, string>> accounts)
        {
            var batches = accounts.Batch(1000).Select(batch =>
            {
                var accountsDataTable = new DataTable();

                accountsDataTable.Columns.Add("AccountId", typeof(long));
                accountsDataTable.Columns.Add("PublicHashedId", typeof(string));

                foreach (var data in batch)
                {
                    accountsDataTable.Rows.Add(data.Key, data.Value);
                }

                return accountsDataTable;
            });

            foreach (var batch in batches)
            {
                await WithConnection(async c =>
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@accounts", batch.AsTableValuedParameter("[employer_account].[AccountPublicHashedIdsTable]"));

                    return await c.ExecuteAsync(
                        sql: "[employer_account].[UpdateAccountPublicHashedIds]",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);
                });
            }
        }
    }
}