using System;
using System.Collections.Generic;
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
    public class PayeRepository : BaseRepository, IPayeRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public PayeRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string reference)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@HashedAccountId", hashedAccountId, DbType.String);
            parameters.Add("@Ref", reference, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<PayeSchemeView>(
                sql: "[employer_account].[GetPayeForAccount_ByRef]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public async Task<Paye> GetPayeSchemeByRef(string payeRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Ref", payeRef, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<Paye>(
                sql: "[employer_account].[GetPaye_ByRef]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public Task UpdatePayeSchemeName(string payeRef, string refName)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Ref", payeRef, DbType.String);
            parameters.Add("@RefName", refName, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpdatePayeName_ByRef]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }


        public async Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<PayeView>(
                sql: "[employer_account].[GetPayeSchemes_ByAccountId]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public Task AddPayeToAccount(Paye payeScheme)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", payeScheme.AccountId, DbType.Int64);
            parameters.Add("@employerRef", payeScheme.EmpRef, DbType.String);
            parameters.Add("@accessToken", payeScheme.AccessToken, DbType.String);
            parameters.Add("@refreshToken", payeScheme.RefreshToken, DbType.String);
            parameters.Add("@addedDate", DateTime.UtcNow, DbType.DateTime);
            parameters.Add("@employerRefName", payeScheme.RefName, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[AddPayeToAccount]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task RemovePayeFromAccount(long accountId, string payeRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AccountId", accountId, DbType.Int64);
            parameters.Add("@PayeRef", payeRef, DbType.String);
            parameters.Add("@RemovedDate", DateTime.UtcNow, DbType.DateTime);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpdateAccountHistory]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}