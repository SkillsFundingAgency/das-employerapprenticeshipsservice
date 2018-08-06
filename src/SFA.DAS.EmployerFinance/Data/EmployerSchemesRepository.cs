using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class EmployerSchemesRepository : BaseRepository, IEmployerSchemesRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public EmployerSchemesRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<PayeSchemes> GetSchemesByEmployerId(long employerId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", employerId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<PayeScheme>(
                sql: "[employer_account].[GetPayeSchemes_ByAccountId]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return new PayeSchemes
            {
                SchemesList = result.ToList()
            };
        }
    }
}