using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class PayeRepository : BaseRepository, IPayeRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _accountDb;
        private readonly Lazy<EmployerFinanceDbContext> _financeDb;

        public PayeRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> accountDb, Lazy<EmployerFinanceDbContext> financeDb)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _accountDb = accountDb;
            _financeDb = financeDb;
        }

        public async Task<Paye> GetPayeSchemeByRef(string payeRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Ref", payeRef, DbType.String);

            var result = await _accountDb.Value.Database.Connection.QueryAsync<Paye>(
                sql: "[employer_account].[GetPaye_ByRef]",
                param: parameters,
                transaction: _accountDb.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public Task UpdatePayeSchemeName(string payeRef, string refName)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Ref", payeRef, DbType.String);
            parameters.Add("@RefName", refName, DbType.String);

            return _accountDb.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpdatePayeName_ByRef]",
                param: parameters,
                transaction: _accountDb.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string reference)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@HashedAccountId", hashedAccountId, DbType.String);
            parameters.Add("@Ref", reference, DbType.String);

            var result = await _accountDb.Value.Database.Connection.QueryAsync<PayeSchemeView>(
                sql: "[employer_account].[GetPayeForAccount_ByRef]",
                param: parameters,
                transaction: _accountDb.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public async Task<PayeSchemes> GetGovernmentGatewayOnlySchemesByEmployerId(long employerId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", employerId, DbType.Int64);

            var result = await _accountDb.Value.Database.Connection.QueryAsync<Paye>(
                sql: "[employer_account].[GetPayeSchemesAddedByGovernmentGateway_ByAccountId]",
                param: parameters,
                transaction: _accountDb.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return new PayeSchemes
            {
                SchemesList = result.ToList()
            };
        }

        public async Task CreatePayeScheme(Paye paye)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", paye.AccountId, DbType.Int64);
            parameters.Add("@empRef", paye.Ref, DbType.String);
            parameters.Add("@name", paye.RefName, DbType.String);
            parameters.Add("@aorn", paye.Aorn, DbType.String);

            await _financeDb.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[CreateAccountPaye]",
                param: parameters,
                transaction: _financeDb.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}
