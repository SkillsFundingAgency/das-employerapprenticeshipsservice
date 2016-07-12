using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class UserAccountRepository : IUserAccountRepository
    {
        readonly string _connectionString = String.Empty;
        public UserAccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Accounts> GetAccountsByUserId(string userId)
        {
            using (var connection = new SqlConnection(_connectionString))
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
