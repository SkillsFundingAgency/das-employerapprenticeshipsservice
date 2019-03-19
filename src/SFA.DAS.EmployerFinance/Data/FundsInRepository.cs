using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class FundsInRepository : BaseRepository, IFundsInRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public FundsInRepository(string connectionString, ILog logger, Lazy<EmployerFinanceDbContext> db) : base(connectionString, logger)
        {
            _db = db;
        }

        public async Task<IEnumerable<LevyFundsIn>> GetFundsIn()
        {
            return await _db.Value.Database.Connection.QueryAsync<LevyFundsIn>(
                "[employer_financial].[GetFundsIn]",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}