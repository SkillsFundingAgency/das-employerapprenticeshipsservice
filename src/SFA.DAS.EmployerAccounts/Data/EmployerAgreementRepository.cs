using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class EmployerAgreementRepository : BaseRepository, IEmployerAgreementRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public EmployerAgreementRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db) 
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<EmployerAgreementView> GetEmployerAgreement(long agreementId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@agreementId", agreementId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<EmployerAgreementView>(
                sql: "[employer_account].[GetEmployerAgreement]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }
    }
}