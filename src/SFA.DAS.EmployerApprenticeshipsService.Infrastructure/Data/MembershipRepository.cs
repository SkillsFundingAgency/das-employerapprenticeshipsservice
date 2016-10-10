using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
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
                    sql: "SELECT * FROM [account].[GetTeamMembers] WHERE AccountId = @accountId AND Email = @email;",
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
                    sql: "SELECT * FROM [account].[Membership] WHERE AccountId = @accountId AND UserId = @userId;",
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
                parameters.Add("@userId", userId, DbType.Int64);
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.ExecuteAsync(
                    sql: "DELETE FROM [account].[Membership] WHERE AccountId = @accountId AND UserId = @userId;",
                    param: parameters,
                    commandType: CommandType.Text);
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
                    sql: "UPDATE [account].[Membership] SET RoleId = @roleId WHERE AccountId = @accountId AND UserId = @userId;",
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
                    sql: "SELECT * FROM [account].[MembershipView] m inner join account.account a on a.id=m.accountid WHERE a.Id = @AccountId AND UserRef = @externalUserId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<MembershipView> GetCaller(string hashedId, string externalUserId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@hashedId", hashedId, DbType.String);
                parameters.Add("@externalUserId", externalUserId, DbType.String);

                return await c.QueryAsync<MembershipView>(
                    sql: "SELECT * FROM [account].[MembershipView] m inner join account.account a on a.id=m.accountid WHERE a.HashedId = @hashedId AND UserRef = @externalUserId;",
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

                return await c.ExecuteAsync(
                    sql: "INSERT INTO [account].[Membership] ([AccountId], [UserId], [RoleId]) VALUES(@accountId, @userId, @roleId); ",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}