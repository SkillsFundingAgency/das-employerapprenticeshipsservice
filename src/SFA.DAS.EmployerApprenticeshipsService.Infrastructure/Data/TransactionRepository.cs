using AutoMapper;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Entities.Transaction;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public TransactionRepository(LevyDeclarationProviderConfiguration configuration, IMapper mapper, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<TransactionLine>> GetAccountTransactionsByDateRange(
            long accountId, DateTime fromDate, DateTime toDate)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@fromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day), DbType.DateTime);
                parameters.Add("@toDate", new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59), DbType.DateTime);

                return await c.QueryAsync<TransactionEntity>(
                    sql: "[employer_financial].[GetTransactionLines_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return MapTransactions(result);
        }

        public async Task<List<TransactionLine>> GetAccountLevyTransactionsByDateRange(
            long accountId, DateTime fromDate, DateTime toDate)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@fromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day), DbType.DateTime);
                parameters.Add("@toDate", new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59), DbType.DateTime);

                return await c.QueryAsync<TransactionEntity>(
                    sql: "[employer_financial].[GetLevyDetail_ByAccountIdAndDateRange]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return MapTransactions(result);
        }

        public async Task<List<TransactionLine>> GetAccountTransactionByProviderAndDateRange(
            long accountId, long ukprn, DateTime fromDate, DateTime toDate)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@ukprn", ukprn, DbType.Int64);
                parameters.Add("@fromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day), DbType.DateTime);
                parameters.Add("@toDate", new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59), DbType.DateTime);

                return await c.QueryAsync<TransactionEntity>(
                    sql: "[employer_financial].[GetPaymentDetail_ByAccountProviderAndDateRange]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return MapTransactions(result);
        }

        public async Task<List<TransactionLine>> GetAccountCoursePaymentsByDateRange(
            long accountId, long ukprn, string courseName, int courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@ukprn", ukprn, DbType.Int64);
                parameters.Add("@courseName", courseName, DbType.String);
                parameters.Add("@courseLevel", courseLevel, DbType.Int32);
                parameters.Add("@pathwayCode", pathwayCode, DbType.Int32);
                parameters.Add("@fromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day), DbType.DateTime);
                parameters.Add("@toDate", new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59), DbType.DateTime);

                return await c.QueryAsync<TransactionEntity>(
                    sql: "[employer_financial].[GetPaymentDetail_ByAccountProviderCourseAndDateRange]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return MapTransactions(result);
        }

        public async Task<int> GetPreviousTransactionsCount(long accountId, DateTime fromDate)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@fromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day), DbType.DateTime);

                return await c.ExecuteScalarAsync<int>(
                    sql: "[employer_financial].[GetPreviousTransactionsCount]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }

        public async Task<List<TransactionSummary>> GetAccountTransactionSummary(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AccountId", accountId, DbType.Int64);

                return await c.QueryAsync<TransactionSummary>(
                    sql: "[employer_financial].[GetTransactionSummary_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
            return result.ToList();
        }

        public async Task<List<TransactionDownloadLine>> GetAllTransactionDetailsForAccountByDate(long accountId, DateTime fromDate, DateTime toDate)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@fromDate", fromDate, DbType.DateTime);
                parameters.Add("@toDate", toDate, DbType.DateTime);

                return await c.QueryAsync<TransactionDownloadLine>(
                    sql: "[employer_financial].[GetAllTransactionDetailsForAccountByDate]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            var hmrcDateService = new HmrcDateService();
            var transactionDownloadLines = result as TransactionDownloadLine[] ?? result.ToArray();
            foreach (var res in transactionDownloadLines)
            {
                if (!string.IsNullOrEmpty(res.PayrollYear) && res.PayrollMonth != 0)
                {
                    res.PeriodEnd = hmrcDateService.GetDateFromPayrollYearMonth(res.PayrollYear, res.PayrollMonth).ToString("MMM yyyy");
                }
            }

            return transactionDownloadLines.OrderByDescending(txn => txn.DateCreated).ToList();
        }

        public async Task CreateTransferTransactions(IEnumerable<TransferTransactionLine> transactions)
        {
            await WithConnection(async c =>
            {
                //var trans = c.BeginTransaction();

                try
                {
                    foreach (var transaction in transactions)
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("@senderAccountId", transaction.AccountId, DbType.Int64);
                        parameters.Add("@receiverAccountId", transaction.ReceiverAccountId, DbType.Int64);
                        parameters.Add("@receiversAccountName", transaction.ReceiverAccountName, DbType.String);
                        parameters.Add("@periodEnd", transaction.PeriodEnd, DbType.String);
                        parameters.Add("@amount", transaction.Amount, DbType.Decimal);
                        parameters.Add("@transactionType", TransactionItemType.Transfer, DbType.Int16);
                        parameters.Add("@transactionDate", transaction.TransactionDate, DbType.DateTime);

                        var result = await c.ExecuteAsync(
                            sql: "[employer_financial].[CreateAccountTransferTransactions]",
                            param: parameters,
                            commandType: CommandType.StoredProcedure);//, transaction: trans);
                    }

                    // trans.Commit();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to save transfer transactions to database");
                    throw;
                }

                return true;
            });
        }

        private List<TransactionLine> MapTransactions(IEnumerable<TransactionEntity> transactionEntities)
        {
            var transactions = new List<TransactionLine>();

            foreach (var entity in transactionEntities)
            {
                switch (entity.TransactionType)
                {
                    case TransactionItemType.Declaration:
                    case TransactionItemType.TopUp:
                        transactions.Add(_mapper.Map<LevyDeclarationTransactionLine>(entity));
                        break;

                    case TransactionItemType.Payment:
                        transactions.Add(_mapper.Map<PaymentTransactionLine>(entity));
                        break;

                    case TransactionItemType.Transfer:
                        transactions.Add(_mapper.Map<TransferTransactionLine>(entity));
                        break;

                    case TransactionItemType.Unknown:
                        transactions.Add(_mapper.Map<TransactionLine>(entity));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return transactions;
        }
    }
}
