using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class MembershipRepository : BaseRepository, IMembershipRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public MembershipRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<TeamMember> Get(long accountId, string email)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@email", email, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<TeamMember>(
                sql: "SELECT * FROM [employer_account].[GetTeamMembers] WHERE AccountId = @accountId AND Email = @email;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<Membership> Get(long userId, long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@userId", userId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<Membership>(
                sql: "SELECT * FROM [employer_account].[Membership] WHERE AccountId = @accountId AND UserId = @userId;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public Task Remove(long userId, long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@UserId", userId, DbType.Int64);
            parameters.Add("@AccountId", accountId, DbType.Int64);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[RemoveMembership]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task ChangeRole(long userId, long accountId, Role role)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int64);
            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@role", role, DbType.Int16);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "UPDATE [employer_account].[Membership] SET Role = @role WHERE AccountId = @accountId AND UserId = @userId;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }

        public async Task<MembershipView> GetCaller(long accountId, string externalUserId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AccountId", accountId, DbType.Int64);
            parameters.Add("@externalUserId", externalUserId, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<MembershipView>(
                sql: "SELECT * FROM [employer_account].[MembershipView] m inner join [employer_account].account a on a.id=m.accountid WHERE a.Id = @AccountId AND UserRef = @externalUserId;",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
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

        public Task Create(long userId, long accountId, Role role)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int64);
            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@role", role, DbType.Int16);
            parameters.Add("@createdDate", DateTime.UtcNow, DbType.DateTime);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "INSERT INTO [employer_account].[Membership] ([AccountId], [UserId], [Role], [CreatedDate]) VALUES(@accountId, @userId, @role, @createdDate); ",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }

        public Task SetShowAccountWizard(string hashedAccountId, string externalUserId, bool showWizard)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@externalUserId", Guid.Parse(externalUserId), DbType.Guid);
            parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
            parameters.Add("@showWizard", showWizard, DbType.Boolean);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpdateShowWizard]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

    }
}