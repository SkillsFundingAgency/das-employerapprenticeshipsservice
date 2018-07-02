using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.Sql.Client;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class PayeRepository : BaseRepository, IPayeRepository
    {
        private readonly Lazy<EmployerAccountDbContext> _db;

        public PayeRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountDbContext> db)
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
    }
}