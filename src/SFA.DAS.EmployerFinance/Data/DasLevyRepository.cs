using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class DasLevyRepository : BaseRepository, IDasLevyRepository
    {
        private readonly EmployerFinanceConfiguration _configuration;
        private readonly Lazy<EmployerFinanceDbContext> _db;
        private readonly ICurrentDateTime _currentDateTime;

        public DasLevyRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db, ICurrentDateTime currentDateTime)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _configuration = configuration;
            _db = db;
            _currentDateTime = currentDateTime;
        }

        public async Task CreateEmployerDeclarations(IEnumerable<DasDeclaration> declarations, string empRef, long accountId)
        {
            foreach (var dasDeclaration in declarations)
            {
                var parameters = new DynamicParameters();

                parameters.Add("@LevyDueYtd", dasDeclaration.LevyDueYtd, DbType.Decimal);
                parameters.Add("@LevyAllowanceForYear", dasDeclaration.LevyAllowanceForFullYear, DbType.Decimal);
                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@EmpRef", empRef, DbType.String);
                parameters.Add("@PayrollYear", dasDeclaration.PayrollYear, DbType.String);
                parameters.Add("@PayrollMonth", dasDeclaration.PayrollMonth, DbType.Int16);
                parameters.Add("@SubmissionDate", dasDeclaration.SubmissionDate, DbType.DateTime);
                parameters.Add("@SubmissionId", dasDeclaration.Id, DbType.Int64);
                parameters.Add("@HmrcSubmissionId", dasDeclaration.SubmissionId, DbType.Int64);
                parameters.Add("@CreatedDate", DateTime.UtcNow, DbType.DateTime);

                if (dasDeclaration.DateCeased.HasValue && dasDeclaration.DateCeased != DateTime.MinValue)
                {
                    parameters.Add("@DateCeased", dasDeclaration.DateCeased, DbType.DateTime);
                }

                if (dasDeclaration.InactiveFrom.HasValue && dasDeclaration.InactiveFrom != DateTime.MinValue)
                {
                    parameters.Add("@InactiveFrom", dasDeclaration.InactiveFrom, DbType.DateTime);
                }

                if (dasDeclaration.InactiveTo.HasValue && dasDeclaration.InactiveTo != DateTime.MinValue)
                {
                    parameters.Add("@InactiveTo", dasDeclaration.InactiveTo, DbType.DateTime);
                }

                parameters.Add("@EndOfYearAdjustment", dasDeclaration.EndOfYearAdjustment, DbType.Boolean);
                parameters.Add("@EndOfYearAdjustmentAmount", dasDeclaration.EndOfYearAdjustmentAmount, DbType.Decimal);
                parameters.Add("@NoPaymentForPeriod", dasDeclaration.NoPaymentForPeriod, DbType.Boolean);

                await _db.Value.Database.Connection.ExecuteAsync(
                    sql: "[employer_financial].[CreateDeclaration]",
                    param: parameters,
                    transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public Task CreateNewPeriodEnd(PeriodEnd periodEnd)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@PeriodEndId", periodEnd.PeriodEndId, DbType.String);
            parameters.Add("@CalendarPeriodMonth", periodEnd.CalendarPeriodMonth, DbType.Int32);
            parameters.Add("@CalendarPeriodYear", periodEnd.CalendarPeriodYear, DbType.Int32);
            parameters.Add("@AccountDataValidAt", periodEnd.AccountDataValidAt, DbType.DateTime);
            parameters.Add("@CommitmentDataValidAt", periodEnd.CommitmentDataValidAt, DbType.DateTime);
            parameters.Add("@CompletionDateTime", periodEnd.CompletionDateTime, DbType.DateTime);
            parameters.Add("@PaymentsForPeriod", periodEnd.PaymentsForPeriod, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[CreatePeriodEnd]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task CreatePayments(IEnumerable<PaymentDetails> payments)
        {
            var batches = payments.Batch(1000).Select(b => b.ToPaymentsDataTable());

            foreach (var batch in batches)
            {
                var parameters = new DynamicParameters();

                parameters.Add("@payments", batch.AsTableValuedParameter("[employer_financial].[PaymentsTable]"));

                await _db.Value.Database.Connection.ExecuteAsync(
                    sql: "[employer_financial].[CreatePayments]",
                    param: parameters,
                    transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<ISet<Guid>> GetAccountPaymentIds(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<Guid>(
                sql: "[employer_financial].[GetAccountPaymentIds]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return new HashSet<Guid>(result);
        }

        public Task<IEnumerable<long>> GetEmployerDeclarationSubmissionIds(string empRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@empRef", empRef, DbType.String);

            return _db.Value.Database.Connection.QueryAsync<long>(
                sql: "[employer_financial].[GetLevyDeclarationSubmissionIdsByEmpRef]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<DasDeclaration> GetLastSubmissionForScheme(string empRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@empRef", empRef, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<DasDeclaration>(
                sql: "[employer_financial].[GetLastLevyDeclarations_ByEmpRef]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public async Task<PeriodEnd> GetLatestPeriodEnd()
        {
            var result = await _db.Value.Database.Connection.QueryAsync<PeriodEnd>(
                sql: "[employer_financial].[GetLatestPeriodEnd]",
                param: null,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public async Task<IEnumerable<PeriodEnd>> GetAllPeriodEnds()
        {
            return await _db.Value.Database.Connection.QueryAsync<PeriodEnd>(
                sql: "[employer_financial].[GetAllPeriodEnds]",
                param: null,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<DasDeclaration> GetSubmissionByEmprefPayrollYearAndMonth(string empRef, string payrollYear, short payrollMonth)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@empRef", empRef, DbType.String);
            parameters.Add("@payrollYear", payrollYear, DbType.String);
            parameters.Add("@payrollMonth", payrollMonth, DbType.Int32);

            var result = await _db.Value.Database.Connection.QueryAsync<DasDeclaration>(
                sql: "[employer_financial].[GetLevyDeclaration_ByEmpRefPayrollMonthPayrollYear]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public async Task<DasDeclaration> GetEffectivePeriod12Declaration(string empRef, string payrollYear, DateTime yearEndAdjustmentCutOff)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@empRef", empRef, DbType.String);
            parameters.Add("@payrollYear", payrollYear, DbType.String);
            parameters.Add("@yearEndAdjustmentCutOff", yearEndAdjustmentCutOff, DbType.DateTime);

            var result = await _db.Value.Database.Connection.QueryAsync<DasDeclaration>(
                sql: "[employer_financial].[GetEffectivePeriod12Declaration]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.SingleOrDefault();
        }

        public Task<decimal> ProcessDeclarations(long accountId, string empRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AccountId", accountId, DbType.Int64);
            parameters.Add("@EmpRef", empRef, DbType.String);
            parameters.Add("@currentDate", _currentDateTime.Now, DbType.DateTime);
            parameters.Add("@expiryPeriod", _configuration.FundsExpiryPeriod, DbType.Int32);

            return _db.Value.Database.Connection.QuerySingleAsync<decimal>(
                sql: "[employer_financial].[ProcessDeclarationsTransactions]",
                param: parameters,
                commandTimeout: 120,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task ProcessPaymentData(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[ProcessPaymentDataTransactions]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<string> FindHistoricalProviderName(long ukprn)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ukprn", ukprn, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<string>(
                sql: "[employer_financial].[GetLastKnownProviderNameForUkprn]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction?.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }

        public async Task<List<LevyDeclarationItem>> GetAccountLevyDeclarations(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<LevyDeclarationItem>(
                sql: "[employer_financial].[GetLevyDeclarations_ByAccountId]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<LevyDeclarationItem>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@payrollYear", payrollYear, DbType.String);
            parameters.Add("@payrollMonth", payrollMonth, DbType.Int16);

            var result = await _db.Value.Database.Connection.QueryAsync<LevyDeclarationItem>(
                sql: "[employer_financial].[GetLevyDeclarations_ByAccountPayrollMonthPayrollYear]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds)
        {
            var accountParametersTable = new AccountIdUserTableParam(accountIds);

            accountParametersTable.Add("@allowancePercentage", _configuration.TransferAllowancePercentage, DbType.Decimal);

            var result = await _db.Value.Database.Connection.QueryAsync<AccountBalance>(
                sql: "[employer_financial].[GetAccountBalance_ByAccountIds]",
                param: accountParametersTable,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public async Task<TransferAllowance> GetTransferAllowance(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@allowancePercentage", _configuration.TransferAllowancePercentage, DbType.Decimal);

            var transferAllowance = await _db.Value.Database.Connection.QueryAsync<TransferAllowance>(
                sql: "[employer_financial].[GetAccountTransferAllowance]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return transferAllowance.SingleOrDefault() ?? new TransferAllowance();
            
        }
    }
}
