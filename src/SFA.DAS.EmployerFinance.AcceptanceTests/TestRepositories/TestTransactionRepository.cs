using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories
{
    public class TestTransactionRepository : BaseRepository, ITestTransactionRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _employerFinanceDbContext;

        public TestTransactionRepository(LevyDeclarationProviderConfiguration configuration,
            ILog logger, Lazy<EmployerFinanceDbContext> employerFinanceDbContext)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _employerFinanceDbContext = employerFinanceDbContext;
        }

        public async Task<int> GetNextAccountId()
        {
            var result = await _employerFinanceDbContext.Value.Database.Connection.QueryFirstAsync<int>(
                sql: "SELECT COALESCE(MAX(AccountId), 0) FROM [employer_financial].LevyDeclaration",
                transaction: _employerFinanceDbContext.Value.Database.CurrentTransaction.UnderlyingTransaction);

            return result + 1;
        }

        public async Task SetTransactionLineDateCreatedToTransactionDate(IEnumerable<long> submissionIds)
        {
            var ids = submissionIds as long[] ?? submissionIds.ToArray();
            var idsDataTable = ids.ToDataTable();
            var parameters = new DynamicParameters();

            parameters.Add("@submissionIds", idsDataTable.AsTableValuedParameter("[employer_financial].[SubmissionIds]"));
            await _employerFinanceDbContext.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[UpdateTransactionLineDateCreatedToTransactionDate_BySubmissionId]",
                param: parameters,
                transaction: _employerFinanceDbContext.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

        }

        public async Task SetTransactionLineDateCreatedToTransactionDate(IDictionary<long, DateTime?> submissionIds)
        {
            var idsDataTable = submissionIds.ToDataTable();
            var parameters = new DynamicParameters();

            parameters.Add("@SubmissionIdsDates", idsDataTable.AsTableValuedParameter("[employer_financial].[SubmissionIdsDate]"));

            await _employerFinanceDbContext.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[UpdateTransactionLinesDateCreated_BySubmissionId]",
                param: parameters,
                transaction: _employerFinanceDbContext.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}