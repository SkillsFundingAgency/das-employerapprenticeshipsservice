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

        public async Task<long> CreateAccount(long userId, string employerNumber, string employerName, string employerRegisteredAddress, DateTime employerDateOfIncorporation, string employerRef, string accessToken, string refreshToken)
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.Int64);
                parameters.Add("@employerNumber", employerNumber, DbType.String);
                parameters.Add("@employerName", employerName, DbType.String);
                parameters.Add("@employerRegisteredAddress", employerRegisteredAddress, DbType.String);
                parameters.Add("@employerDateOfIncorporation", employerDateOfIncorporation, DbType.DateTime);
                parameters.Add("@employerRef", employerRef, DbType.String);
                parameters.Add("@accountId", null, DbType.Int64, ParameterDirection.Output, 8);
                parameters.Add("@accessToken", Guid.Parse(accessToken), DbType.Guid);
                parameters.Add("@refreshToken", Guid.Parse(refreshToken), DbType.Guid);

                var trans = c.BeginTransaction();
                await c.ExecuteAsync(
                    sql: "[dbo].[CreateAccount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);
                trans.Commit();
                
                return parameters.Get<long>("@accountId");
            });
        }

        public async Task AddPayeToAccountForExistingLegalEntity(long accountId, long legalEntityId, string employerRef, string accessToken, string refreshToken)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);
                parameters.Add("@employerRef", employerRef, DbType.String);
                parameters.Add("@accessToken", Guid.Parse(accessToken), DbType.Guid);
                parameters.Add("@refreshToken", Guid.Parse(refreshToken), DbType.Guid);

                var trans = c.BeginTransaction();
                var result = await c.ExecuteAsync(
                    sql: "[dbo].[AddPayeToAccountForExistingLegalEntity]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);
                trans.Commit();
                return result;
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