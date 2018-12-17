using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class AccountTeamRepository : BaseRepository, IAccountTeamRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public AccountTeamRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedAccountId, string externalUserId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
            parameters.Add("@externalUserId", externalUserId, DbType.String);

            const string sql = @"select tm.* from [employer_account].[GetTeamMembers] tm 
                join [employer_account].[Membership] m on m.AccountId = tm.AccountId
                join [employer_account].[User] u on u.Id = m.UserId
                where u.UserRef = @externalUserId and tm.hashedId = @hashedAccountId";

            var result = await _db.Value.Database.Connection.QueryAsync<TeamMember>(
                sql: sql,
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.ToList();
        }

        public async Task<TeamMember> GetMember(string hashedAccountId, string email)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
            parameters.Add("@email", email, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<TeamMember>(
                sql: "SELECT * FROM [employer_account].[GetTeamMembers] WHERE IsUser = 1 AND HashedId = @hashedAccountId AND Email = @email;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<ICollection<TeamMember>> GetAccountTeamMembers(string hashedAccountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<TeamMember>(
                sql: "[employer_account].[GetEmployerAccountMembers]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
    }
}