using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class UserAccountRepository : BaseRepository, IUserAccountRepository
    {
        public UserAccountRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<Accounts> GetAccountsByUserId(string userId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.String);

                return await c.QueryAsync<Account>(
                    sql: @"select a.* from [dbo].[User] u 
                        left join[dbo].[Membership] m on m.UserId = u.Id
                        left join[dbo].[Account]  a on m.AccountId = a.Id
                        where u.PireanKey = @Id",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return new Accounts { AccountList = (List<Account>)result };
        }

        public async Task<User> Get(string email)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", email, DbType.String);

                return await c.QueryAsync<User>(
                    sql: "SELECT * FROM [dbo].[User] WHERE Email = @email;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.FirstOrDefault();
        }
    }
}
