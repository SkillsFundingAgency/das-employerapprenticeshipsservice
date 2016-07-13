using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(string connectionString)
            : base(connectionString)
        {
        }

        public async Task CreateAccount(string userRef, string employerNumber, string employerName, string employerRef)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("userRef", new Guid(userRef), DbType.Guid);
                parameters.Add("employerNumber", employerNumber, DbType.String);
                parameters.Add("employerName", employerName, DbType.String);
                parameters.Add("employerRef", employerRef, DbType.String);

                var returnValue = await c.ExecuteAsync(
                    sql: "[dbo].[CreateAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return returnValue;
            });
        }
    }
}