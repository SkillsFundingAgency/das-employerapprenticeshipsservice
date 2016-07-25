using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class EmployerAccountRepository : BaseRepository, IEmployerAccountRepository
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IConfigurationService _configurationService;

        public EmployerAccountRepository(EmployerApprenticeshipsServiceConfiguration configuration)
            :base(configuration)
        {
        }

        public async Task<Account> GetAccountById(int id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);

                return await c.QueryAsync<Account>(
                    sql: "select a.* from [dbo].[Account] a where a.Id = @Id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return result.FirstOrDefault();
        }
    }
}

