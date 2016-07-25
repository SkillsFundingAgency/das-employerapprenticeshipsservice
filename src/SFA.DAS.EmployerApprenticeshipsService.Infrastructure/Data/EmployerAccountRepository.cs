using System.Data.SqlClient;
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
        public override string ConnectionString { get; set; }

        public EmployerAccountRepository(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Account> GetAccountById(int id)
        {
            ConnectionString = _configuration.Employer.DatabaseConnectionString;

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                var sql = @"select a.* from [dbo].[Account] a where a.Id = @Id";
                var account = await connection.QueryFirstOrDefaultAsync<Account>(sql, new { Id = id });

                connection.Close();
                 return account;
                //return new Account { Id = account.Id, Name = account.Name};
            }

        }
    }
}

