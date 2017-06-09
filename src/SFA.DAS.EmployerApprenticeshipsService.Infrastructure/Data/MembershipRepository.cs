using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class MembershipRepository : BaseRepository, IMembershipRepository
    {
        public MembershipRepository(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        public async Task<TeamMember> Get(long accountId, string email)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@email", email, DbType.String);

                return await c.QueryAsync<TeamMember>(
                    sql: "SELECT * FROM [employer_account].[GetTeamMembers] WHERE AccountId = @accountId AND Email = @email;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<Membership> Get(long userId, long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@userId", userId, DbType.Int64);

                return await c.QueryAsync<Membership>(
                    sql: "SELECT * FROM [employer_account].[Membership] WHERE AccountId = @accountId AND UserId = @userId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task Remove(long userId, long accountId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId, DbType.Int64);
                parameters.Add("@AccountId", accountId, DbType.Int64);

                return await c.ExecuteAsync(
                    sql: "[employer_account].[RemoveMembership]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task ChangeRole(long userId, long accountId, short roleId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.Int64);
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@roleId", roleId, DbType.Int16);

                return await c.ExecuteAsync(
                    sql: "UPDATE [employer_account].[Membership] SET RoleId = @roleId WHERE AccountId = @accountId AND UserId = @userId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<MembershipView> GetCaller(long accountId, string externalUserId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@externalUserId", externalUserId, DbType.String);

                return await c.QueryAsync<MembershipView>(
                    sql: "SELECT * FROM [employer_account].[MembershipView] m inner join [employer_account].account a on a.id=m.accountid WHERE a.Id = @AccountId AND UserRef = @externalUserId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
                parameters.Add("@externalUserId", externalUserId, DbType.String);

                return await c.QueryAsync<MembershipView>(
                    sql: "SELECT * FROM [employer_account].[MembershipView] m inner join [employer_account].account a on a.id=m.accountid WHERE a.HashedId = @hashedAccountId AND UserRef = @externalUserId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task Create(long userId, long accountId, short roleId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.Int64);
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@roleId", roleId, DbType.Int16);
                parameters.Add("@createdDate",DateTime.UtcNow, DbType.DateTime);

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [employer_account].[Membership] ([AccountId], [UserId], [RoleId], [CreatedDate]) VALUES(@accountId, @userId, @roleId, @createdDate); ",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}