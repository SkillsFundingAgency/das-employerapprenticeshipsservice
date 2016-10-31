﻿using System.Collections.Generic;
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

        public async Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedId, string externalUserId)
        {
            var result = await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@hashedId", hashedId, DbType.String);
                parameters.Add("@externalUserId", externalUserId, DbType.String);

                const string sql = @"select tm.* from [account].[GetTeamMembers] tm 
                            join [account].[Membership] m on m.AccountId = tm.AccountId
                            join [account].[User] u on u.Id = m.UserId
                            where u.PireanKey = @externalUserId and tm.hashedId = @hashedId";
                return await connection.QueryAsync<TeamMember>(
                    sql: sql,
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }

        public async Task<TeamMember> GetMember(string hashedId, string email)
        {
            var result = await WithConnection(async connection =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@hashedId", hashedId, DbType.String);
                parameters.Add("@email", email, DbType.String);

                return await connection.QueryAsync<TeamMember>(
                    sql: "SELECT * FROM [account].[GetTeamMembers] WHERE HashedId = @hashedId AND Email = @email;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }
    }
}
