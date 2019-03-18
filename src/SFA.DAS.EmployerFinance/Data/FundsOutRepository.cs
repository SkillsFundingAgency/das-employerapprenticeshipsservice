using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class FundsOutRepository : BaseRepository, IFundsOutRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public FundsOutRepository(string connectionString, ILog logger, Lazy<EmployerFinanceDbContext> db) : base(connectionString, logger)
        {
            _db = db;
        }

        public async Task<IEnumerable<PaymentFundsOut>> GetFundsOut()
        {
            return await _db.Value.Database.Connection.QueryAsync<PaymentFundsOut>(
                "[employer_financial].[GetFundsOut]",
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}