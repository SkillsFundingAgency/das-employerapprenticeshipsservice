using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class EmployerAccountRepository : BaseRepository, IEmployerAccountRepository
    {
        
        public EmployerAccountRepository(EmployerApprenticeshipsServiceConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<Account> GetAccountById(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int64);

                return await c.QueryAsync<Account>(
                    sql: "select a.* from [account].[Account] a where a.Id = @id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return result.SingleOrDefault();
        }

        public async Task<Account> GetAccountByHashedId(string hashedId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HashedId", hashedId, DbType.String);

                return await c.QueryAsync<Account>(
                    sql: "select a.* from [account].[Account] a where a.HashedId = @HashedId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return result.SingleOrDefault();
        }

        public async Task<Accounts> GetAccounts()
        {
            var result = await WithConnection(async c => await c.QueryAsync<Account>(
                sql: "select a.* from [account].[Account] a;",
                commandType: CommandType.Text));
            return new Accounts() {AccountList = result.ToList()};
        }
    }
}

