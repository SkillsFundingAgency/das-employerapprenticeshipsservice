using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        private readonly ILog _logger;
        private readonly EmployerFinanceConfiguration _configuration;

        public TestTransactionRepository(EmployerFinanceConfiguration configuration,
            ILog logger, Lazy<EmployerFinanceDbContext> employerFinanceDbContext)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _employerFinanceDbContext = employerFinanceDbContext;
            _logger = logger;
            _configuration = configuration;
        }

        public Task<int> GetMaxAccountId()
        {
            return _employerFinanceDbContext.Value.Database.Connection.QueryFirstAsync<int>(
                sql: "SELECT COALESCE(MAX(AccountId), 0) FROM [employer_financial].LevyDeclaration",
                transaction: _employerFinanceDbContext.Value.Database.CurrentTransaction.UnderlyingTransaction);
        }

        public Task RemovePayeRef(string empRef)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@empref", empRef, DbType.String);

            return RunOutsideTxn(conn => 
                conn.ExecuteAsync(
                        sql: "[employer_financial].[DeleteSubmissions_ByEmpRef]",
                        param: parameters,
                        commandType: CommandType.StoredProcedure));
        }

        private async Task RunOutsideTxn(Func<SqlConnection, Task> command)
        {
            var conn = new SqlConnection(_configuration.DatabaseConnectionString);
            try
            {
                await conn.OpenAsync();
                await command(conn);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in Acceptance Tests - ClearDownPayeRefsFromDbAsync");
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public Task SetTransactionLineDateCreatedToTransactionDate(IEnumerable<long> submissionIds)
        {
            var ids = submissionIds as long[] ?? submissionIds.ToArray();
            var idsDataTable = ids.ToDataTable();
            var parameters = new DynamicParameters();

            parameters.Add("@submissionIds", idsDataTable.AsTableValuedParameter("[employer_financial].[SubmissionIds]"));
            return _employerFinanceDbContext.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[UpdateTransactionLineDateCreatedToTransactionDate_BySubmissionId]",
                param: parameters,
                transaction: _employerFinanceDbContext.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task SetTransactionLineDateCreatedToTransactionDate(IDictionary<long, DateTime?> submissionIds)
        {
            var idsDataTable = submissionIds.ToDataTable();
            var parameters = new DynamicParameters();

            parameters.Add("@SubmissionIdsDates", idsDataTable.AsTableValuedParameter("[employer_financial].[SubmissionIdsDate]"));

            return _employerFinanceDbContext.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[UpdateTransactionLinesDateCreated_BySubmissionId]",
                param: parameters,
                transaction: _employerFinanceDbContext.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}