using System;
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
    public class InvitationRepository : BaseRepository, IInvitationRepository
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        public override string ConnectionString { get; set; }

        public InvitationRepository(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<InvitationView>> Get(string userId)
        {
            ConnectionString = _configuration.Employer.DatabaseConnectionString;

            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.String);

                return await c.QueryAsync<InvitationView>(
                    sql: "SELECT * FROM [dbo].[GetInvitations] WHERE ExternalUserId = @userId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }
    }
}