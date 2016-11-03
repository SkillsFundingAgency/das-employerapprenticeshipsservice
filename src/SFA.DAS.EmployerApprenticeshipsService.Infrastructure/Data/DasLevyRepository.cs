using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class DasLevyRepository : BaseRepository, IDasLevyRepository
    {
        public DasLevyRepository(LevyDeclarationProviderConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.String);
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasDeclaration>(
                    sql: "SELECT LevyDueYtd, SubmissionId AS Id, SubmissionDate AS [Date] FROM [levy].[LevyDeclaration] WHERE empRef = @EmpRef and SubmissionId = @Id;",
                    param: parameters,
                    commandType: CommandType.Text);
            });

            return result.SingleOrDefault();
        }

        public async Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef, long accountId)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@LevyDueYtd", dasDeclaration.LevyDueYtd, DbType.Decimal);
                parameters.Add("@LevyAllowanceForYear", dasDeclaration.LevyAllowanceForFullYear, DbType.Decimal);
                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@EmpRef", empRef, DbType.String);
                parameters.Add("@PayrollYear", dasDeclaration.PayrollYear, DbType.String);
                parameters.Add("@PayrollMonth", dasDeclaration.PayrollMonth, DbType.Int16);
                parameters.Add("@SubmissionDate", dasDeclaration.Date, DbType.DateTime);
                parameters.Add("@SubmissionId", dasDeclaration.Id, DbType.String);
                
                return await c.ExecuteAsync(
                    sql: "[levy].[CreateDeclaration]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<LevyDeclarationView>(
                    sql: "[levy].[GetLevyDeclarations_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task<DasDeclaration> GetLastSubmissionForScheme(string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasDeclaration>(
                    sql: "[levy].[GetLastLevyDeclarations_ByEmpRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task ProcessDeclarations()
        {
            await WithConnection(async c => await c.ExecuteAsync(
                sql: "[levy].[ProcessDeclarationsTransactions]",
                param: null,
                commandType: CommandType.StoredProcedure));
        }

        public async Task<List<TransactionLine>> GetTransactions(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<TransactionLine>(
                    sql: "[levy].[GetTransactionLines_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }
        
        public async Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds)
        {
            var result = await WithConnection(async c =>
            {

                var dataTable = new DataTable("[levy].[AccountIds]");
                dataTable.Columns.Add("AccountId", typeof(long));
                foreach (var item in accountIds)
                {
                    dataTable.Rows.Add(item);
                }

                var parameters = new AccountIdUserTableParam(accountIds);
                
                return await c.QueryAsync<AccountBalance>(
                 "[levy].[GetAccountBalance_ByAccountIds]",
                 parameters,
                 commandType: CommandType.StoredProcedure);
            });
            

            return result.ToList();
            
        }
    }
}

