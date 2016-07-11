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
        public async Task<List<Account>> GetAccountsByUserId(string userId)
        {
            var connection = new SqlConnection(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=SFA.DAS.EmployerApprenticeshipsService.Database;Integrated Security=True;Pooling=False;Connect Timeout=30");
            var sql = @"select a.* from [dbo].[User] u 
                        left join[dbo].[Membership]
                        m on m.UserId = u.Id
                        left join[dbo].[Account]
                        a on m.AccountId = a.Id
                        where u.PireanKey = @Id";
           var accounts =  connection.Query<Account>(sql, new {Id = userId});
            return (List<Account>) accounts;
        }
    }
}
