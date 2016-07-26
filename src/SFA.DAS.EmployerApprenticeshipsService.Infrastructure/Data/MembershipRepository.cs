using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class MembershipRepository : BaseRepository
    {
        public MembershipRepository(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
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

    }
}