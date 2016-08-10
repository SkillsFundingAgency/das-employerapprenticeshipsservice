using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
        }

        public async Task<long> CreateAccount(string userRef, string employerNumber, string employerName, string employerRef)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", new Guid(userRef), DbType.Guid);
                parameters.Add("@employerNumber", employerNumber, DbType.String);
                parameters.Add("@employerName", employerName, DbType.String);
                parameters.Add("@employerRef", employerRef, DbType.String);
                parameters.Add("@accountId", null, DbType.Int64, ParameterDirection.Output, 8);

                await c.ExecuteAsync(
                    sql: "[dbo].[CreateAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<long>("@accountId");
            });
        }

        public async Task<List<PayeView>> GetPayeSchemes(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<PayeView>(
                    sql: "SELECT * FROM [dbo].[GetAccountPayeSchemes] WHERE AccountId = @accountId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }
    }
}