using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.AccountTeam;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public class MembershipRepository : BaseRepository, IMembershipRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public MembershipRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
            parameters.Add("@externalUserId", externalUserId, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<MembershipView>(
                sql: "[employer_account].[GetTeamMember]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }
    }
}
