using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class MembershipRepository : BaseRepository, IMembershipRepository
    {
        public MembershipRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger) : base(configuration,logger)
        {
        }

        public async Task<TeamMember> Get(long accountId, string email)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int32);
                parameters.Add("@email", email, DbType.String);

                return await c.QueryAsync<TeamMember>(
                    sql: "SELECT * FROM [dbo].[GetTeamMembers] WHERE AccountId = @accountId AND Email = @email;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.FirstOrDefault();
        }

        public async Task<Membership> Get(long userId, long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int32);
                parameters.Add("@userId", userId, DbType.Int32);

                return await c.QueryAsync<Membership>(
                    sql: "SELECT * FROM [dbo].[Membership] WHERE AccountId = @accountId AND UserId = @userId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.FirstOrDefault();
        }

        public async Task Remove(long userId, long accountId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.Int32);
                parameters.Add("@accountId", accountId, DbType.Int32);

                return await c.ExecuteAsync(
                    sql: "DELETE FROM [dbo].[Membership] WHERE AccountId = @accountId AND UserId = @userId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}