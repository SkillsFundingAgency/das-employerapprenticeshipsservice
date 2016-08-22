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
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class UserAccountRepository : BaseRepository, IUserAccountRepository
    {
        public UserAccountRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
        }

        public async Task<Accounts> GetAccountsByUserId(string userId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", Guid.Parse(userId), DbType.Guid);

                return await c.QueryAsync<Account>(
                    sql: @"[account].[GetAccounts_ByUserId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
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
                    sql: "SELECT Id, CONVERT(NVARCHAR(50), PireanKey) AS UserRef, Email FROM [account].[User] WHERE Email = @email;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task<User> Get(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int64);

                return await c.QueryAsync<User>(
                    sql: "SELECT Id, CONVERT(NVARCHAR(50), PireanKey) AS UserRef, Email FROM [account].[User] WHERE Id = @id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }
    }
}
