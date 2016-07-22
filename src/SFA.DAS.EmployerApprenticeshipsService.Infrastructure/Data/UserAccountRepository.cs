using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        
        public UserAccountRepository(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Accounts> GetAccountsByUserId(string userId)
        {
            using (var connection = new SqlConnection(_configuration.Employer.DatabaseConnectionString))
            {
                await connection.OpenAsync();

                var sql = @"select a.* from [dbo].[User] u 
                        left join[dbo].[Membership] m on m.UserId = u.Id
                        left join[dbo].[Account]  a on m.AccountId = a.Id
                        where u.PireanKey = @Id";
                var accounts = connection.Query<Account>(sql, new { Id = userId });

                connection.Close();
                return new Accounts { AccountList = (List<Account>)accounts };
            }
       
        }
    }
}
