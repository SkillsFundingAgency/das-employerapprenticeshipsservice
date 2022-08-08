using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class PaymentFundsOutRepository : BaseRepository, IPaymentFundsOutRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public PaymentFundsOutRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<IEnumerable<PaymentFundsOut>> GetPaymentFundsOut(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AccountId", accountId, DbType.Int64);

            return await _db.Value.Database.Connection.QueryAsync<PaymentFundsOut>(
                "[employer_financial].[GetPaymentFundsOut]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}