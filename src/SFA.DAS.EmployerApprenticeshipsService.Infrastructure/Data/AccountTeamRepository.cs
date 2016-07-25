using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class AccountTeamRepository : BaseRepository, IAccountTeamRepository
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        public override string ConnectionString { get; set; }

        public AccountTeamRepository(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;

            ConnectionString = _configuration.Employer.DatabaseConnectionString;
        }
        public async Task<List<TeamMember>> GetAccountTeamMembersForUserId(int accountId, string userId)
        {
            return await WithConnection(async connection =>
            {
                var sql = @"select tm.* from [GetTeamMembers] tm 
join [Membership] m on m.AccountId = tm.AccountId
join [User] u on u.Id = m.UserId
where u.PireanKey = @userId and tm.AccountId = @accountId";
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
