using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class AccountTeamRepository : BaseRepository, IAccountTeamRepository
    {
        public AccountTeamRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<TeamMember>> GetAccountTeamMembersForUserId(int accountId, string userId)
        {
            return await WithConnection(async connection =>
            {
                var sql = @"select u.Id , u.Email,CONVERT(varchar(64), u.PireanKey) as 'UserRef', r.Name as 'Role' from [User] u
                            left join [Membership] m on m.UserId = u.Id
                            left join [Role] r on r.Id = m.RoleId
                            left join [Account] a on a.Id = m.AccountId
                            where u.PireanKey = @userId and a.Id = @accountId";
                var members = await connection.QueryAsync<TeamMember>(sql, new { accountId = accountId, userId = userId });
                return new List<TeamMember>(members);
            });
        }

        public async Task<Membership> GetMembership(long accountId, string userId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int32);
                parameters.Add("@userId", userId, DbType.String);

                return await c.QueryAsync<Membership>(
                    sql: "SELECT m.* FROM [dbo].[Membership] m INNER JOIN [dbo].[User] u ON u.Id = m.UserId WHERE m.AccountId = @accountId AND u.PireanKey = @userId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.FirstOrDefault();
        }
    }
}
