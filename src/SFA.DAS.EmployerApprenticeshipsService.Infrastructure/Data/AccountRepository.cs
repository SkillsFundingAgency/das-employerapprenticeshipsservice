using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(EmployerApprenticeshipsServiceConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<long> CreateAccount(long userId, string employerNumber, string employerName, string employerRegisteredAddress, DateTime employerDateOfIncorporation, string employerRef, string accessToken, string refreshToken, bool signAgreement)
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
                parameters.Add("@accessToken", accessToken, DbType.String);
                parameters.Add("@refreshToken", refreshToken, DbType.String);
                parameters.Add("@addedDate",DateTime.UtcNow,DbType.DateTime);
                parameters.Add("@signAgreement", signAgreement, DbType.Boolean);


                var trans = c.BeginTransaction();
                await c.ExecuteAsync(
                    sql: "[account].[CreateAccount]",
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
                var storedProcedureName = await GetPayeStoredProcedureForUpdateOrCreate(employerRef, c);

                var parameters = new DynamicParameters();
                parameters.Add("@legalEntityId", legalEntityId, DbType.Int64);
                parameters.Add("@employerRef", employerRef, DbType.String);
                parameters.Add("@accessToken", accessToken, DbType.String);
                parameters.Add("@refreshToken",refreshToken, DbType.String);

                var trans = c.BeginTransaction();
                
                var result = await c.ExecuteAsync(
                    sql: storedProcedureName,
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);

                var accountHistoryParams = new DynamicParameters();
                accountHistoryParams.Add("@AccountId", accountId, DbType.Int64);
                accountHistoryParams.Add("@PayeRef", employerRef, DbType.String);
                accountHistoryParams.Add("@AddedDate", DateTime.UtcNow, DbType.DateTime);

                await c.ExecuteAsync(
                    sql: "[account].[CreateAccountHistory]",
                    param: accountHistoryParams,
                    commandType: CommandType.StoredProcedure, transaction: trans);


                trans.Commit();
                return result;
            });
        }

        public async Task RemovePayeFromAccount(long accountId, string payeRef)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@PayeRef", payeRef, DbType.String);
                parameters.Add("@RemovedDate", DateTime.UtcNow, DbType.DateTime);
                
                var result = await c.ExecuteAsync(
                   sql: "[account].[UpdateAccountHistory]",
                   param: parameters,
                   commandType: CommandType.StoredProcedure);

                return result;
            });
        }

        public async Task AddPayeToAccountForNewLegalEntity(Paye payeScheme, LegalEntity legalEntity)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", payeScheme.AccountId, DbType.Int64);
                parameters.Add("@companyNumber", legalEntity.Code, DbType.String);
                parameters.Add("@companyName", legalEntity.Name, DbType.String);
                parameters.Add("@CompanyAddress", legalEntity.RegisteredAddress, DbType.String);
                parameters.Add("@CompanyDateOfIncorporation", legalEntity.DateOfIncorporation, DbType.DateTime);
                parameters.Add("@employerRef", payeScheme.EmpRef, DbType.String);
                parameters.Add("@accessToken", payeScheme.AccessToken, DbType.String);
                parameters.Add("@refreshToken", payeScheme.RefreshToken, DbType.String);
                parameters.Add("@addedDate", DateTime.UtcNow, DbType.DateTime);

                var trans = c.BeginTransaction();
                var result = await c.ExecuteAsync(
                    sql: "[account].[AddPayeToAccountForNewLegalEntity]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);
                trans.Commit();
                return result;
            });
        }

        public async Task<EmployerAgreementView> CreateLegalEntity(
            long accountId, LegalEntity legalEntity, bool signAgreement, DateTime signedDate, long signedById) 
        {
            return await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@companyNumber", legalEntity.Code, DbType.String);
                parameters.Add("@companyName", legalEntity.Name, DbType.String);
                parameters.Add("@CompanyAddress", legalEntity.RegisteredAddress, DbType.String);
                parameters.Add("@CompanyDateOfIncorporation", legalEntity.DateOfIncorporation, DbType.DateTime);
                parameters.Add("@signAgreement", signAgreement, DbType.Boolean);
                parameters.Add("@signedDate", signedDate, DbType.DateTime);
                parameters.Add("@signedById", signedById, DbType.Int64);
                parameters.Add("@legalEntityId", signedById, DbType.Int64);
                parameters.Add("@employerAgreementId", signedById, DbType.Int64);

                var trans = c.BeginTransaction();
                var result = await c.ExecuteAsync(
                    sql: "[account].[CreateLegalEntityWithAgreement]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure, transaction: trans);
                trans.Commit();

                var legalEntityId = parameters.Get<long>("@legalEntityId");
                var agreementId = parameters.Get<long>("@employerAgreementId");

                return new EmployerAgreementView
                {
                    Id = agreementId,
                    AccountId = accountId,
                    LegalEntityId = legalEntityId,
                    LegalEntityName = legalEntity.Name,
                    LegalEntityCode = legalEntity.Code,
                    LegalEntityRegisteredAddress = legalEntity.RegisteredAddress,
                    LegalEntityIncorporatedDate = legalEntity.DateOfIncorporation,
                    Status = EmployerAgreementStatus.Pending,
                };
            });
        }

        public async Task<List<PayeView>> GetPayeSchemes(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<PayeView>(
                    sql: "SELECT * FROM [account].[GetAccountPayeSchemes] WHERE AccountId = @accountId;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.ToList();
        }

        public async Task<List<EmployerAgreementView>> GetEmployerAgreementsLinkedToAccount(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<EmployerAgreementView>(
                    sql: "account.GetEmployerAgreementsLinkedToAccount",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        private static async Task<string> GetPayeStoredProcedureForUpdateOrCreate(string employerRef, IDbConnection c)
        {
            var searchParams = new DynamicParameters();
            searchParams.Add("@employerRef", employerRef, DbType.String);

            var payeResult =
                await
                    c.QueryAsync<int>("select 1 from [account].[paye] where Ref = @employerRef", searchParams,
                        commandType: CommandType.Text);
            var storedProcedureName = "[account].[CreatePaye]";
            if (payeResult.SingleOrDefault() == 1)
            {
                storedProcedureName = "[account].[UpdatePaye]";
            }
            return storedProcedureName;
        }
    }
}