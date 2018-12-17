﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class EmployerAccountTeamRepository : BaseRepository, IEmployerAccountTeamRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public EmployerAccountTeamRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
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

        public async Task<TeamMember> GetMember(string hashedAccountId, string email, bool onlyIfMemberIsActive)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
            parameters.Add("@email", email, DbType.String);
            parameters.Add("@onlyIfMemberIsActive", onlyIfMemberIsActive, DbType.Boolean);

            var result = await _db.Value.Database.Connection.QueryAsync<TeamMember>(
                sql: @"SELECT TOP 1 * FROM [employer_account].[GetTeamMembers] "+
                      "WHERE HashedId = @hashedAccountId "+
                      "AND Email = @email "+
                      "AND (@onlyIfMemberIsActive = 0 OR IsUser = 1) "+
                      "ORDER BY IsUser DESC;",
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