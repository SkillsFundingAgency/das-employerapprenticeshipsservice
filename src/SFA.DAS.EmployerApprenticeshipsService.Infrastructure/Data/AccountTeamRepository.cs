using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        }
        public async Task<List<TeamMember>> GetAccountTeamMembersForUserId(int accountId, string userId)
        {
            ConnectionString = _configuration.Employer.DatabaseConnectionString;

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



    }
}
