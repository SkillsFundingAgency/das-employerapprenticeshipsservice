using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class EmployerSchemesRepository : BaseRepository, IEmployerSchemesRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public EmployerSchemesRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.SqlConnectionString, logger)
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

        public async Task<PayeScheme> GetSchemeByRef(string empref)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@payeRef", empref, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<PayeScheme>(
                sql: "[employer_account].[GetPayeSchemesInUse]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }
    }
}