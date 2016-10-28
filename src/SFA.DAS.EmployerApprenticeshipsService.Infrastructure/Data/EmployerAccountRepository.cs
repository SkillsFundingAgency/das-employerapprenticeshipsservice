using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Infrastructure.Data
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

        public async Task<Accounts> GetAccounts(string toDate, int pageNumber, int pageSize)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@toDate", toDate);
            var offset = pageSize * (pageNumber - 1);

            var countResult = await WithConnection(async c=> await c.QueryAsync<int>(sql: $"select count(*) from [account].[Account] a;"));

            var result = await WithConnection(async c => await c.QueryAsync<Account>(
                sql:    $"select a.* from [account].[Account] a ORDER BY a.Id OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY;", 
                commandType: CommandType.Text));

            return new Accounts() {AccountsCount = countResult.First(), AccountList = result.ToList()};
        }
    }
}

