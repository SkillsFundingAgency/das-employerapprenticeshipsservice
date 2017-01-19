using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class AccountTeamRepository : BaseRepository, IAccountTeamRepository
    {
        public AccountTeamRepository(EmployerApprenticeshipsServiceConfiguration configuration )
            :base(configuration)
        {
        }

        public async Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedAccountId, string externalUserId)
        {
            var result = await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
                parameters.Add("@externalUserId", externalUserId, DbType.String);

                const string sql = @"select tm.* from [employer_account].[GetTeamMembers] tm 
                            join [employer_account].[Membership] m on m.AccountId = tm.AccountId
                            join [employer_account].[User] u on u.Id = m.UserId
                            where u.PireanKey = @externalUserId and tm.hashedId = @hashedAccountId";
                return await connection.QueryAsync<TeamMember>(
                    sql: sql,
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }

        public async Task<TeamMember> GetMember(string hashedAccountId, string email)
        {
            var result = await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
                parameters.Add("@email", email, DbType.String);

                return await connection.QueryAsync<TeamMember>(
                    sql: "SELECT * FROM [employer_account].[GetTeamMembers] WHERE HashedId = @hashedAccountId AND Email = @email;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }
    }
}
