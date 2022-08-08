using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class LevyFundsInRepository : BaseRepository, ILevyFundsInRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public LevyFundsInRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<IEnumerable<LevyFundsIn>> GetLevyFundsIn(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AccountId", accountId, DbType.Int64);

            return await _db.Value.Database.Connection.QueryAsync<LevyFundsIn>(
                "[employer_financial].[GetLevyFundsIn]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}