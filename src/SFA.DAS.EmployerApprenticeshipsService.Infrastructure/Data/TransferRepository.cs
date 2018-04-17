using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransferRepository : BaseRepository, ITransferRepository
    {
        private readonly decimal _allowancePercentage;
        private readonly ILog _logger;
        public TransferRepository(LevyDeclarationProviderConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _logger = logger;
            _allowancePercentage = configuration.TransferAllowancePercentage;

        }

        public async Task CreateAccountTransfers(IEnumerable<AccountTransfer> transfers)
        {
            await WithTransaction(async (connection, transaction) =>
            {
                var accountTransfers = transfers as AccountTransfer[] ?? transfers.ToArray();

                try
                {
                    var transferDataTable = CreateTransferDataTable(accountTransfers);

                    var parameters = new DynamicParameters();
                    parameters.Add("@transfers", transferDataTable.AsTableValuedParameter("[employer_financial].[AccountTransferTable]"));

                    await connection.ExecuteAsync(
                        sql: "[employer_financial].[CreateAccountTransfer]",
                        param: parameters,
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    var transfer = accountTransfers.FirstOrDefault();
                    _logger.Error(ex, $"Failed to save transfers for account id {transfer?.SenderAccountId}");

                    throw;
                }
            });
        }

        public async Task<decimal> GetTransferAllowance(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@allowancePercentage", _allowancePercentage, DbType.Single);

                return await c.QuerySingleOrDefaultAsync<decimal?>(
                    sql: "[employer_financial].[GetAccountTransferAllowance]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result ?? 0;
        }

        public async Task<IEnumerable<AccountTransfer>> GetAccountTransfersByPeriodEnd(long receiverAccountId, string periodEnd)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@receiverAccountId", receiverAccountId, DbType.Int64);
                parameters.Add("@periodEnd", periodEnd, DbType.String);

                return await c.QueryAsync<AccountTransfer>(
                    sql: "[employer_financial].[GetAccountTransfersByPeriodEnd]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;


        }

        public async Task<AccountTransferDetails> GetTransferPaymentDetails(AccountTransfer transfer)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@receiverAccountId", transfer.ReceiverAccountId, DbType.Int64);
                parameters.Add("@periodEnd", transfer.PeriodEnd, DbType.String);
                parameters.Add("@apprenticeshipId", transfer.ApprenticeshipId, DbType.Int64);

                return await c.QuerySingleOrDefaultAsync<AccountTransferDetails>(
                    sql: "[employer_financial].[GetTransferPaymentDetails]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }

        public async Task<IEnumerable<AccountTransfer>> GetReceiverAccountTransfersByPeriodEnd(long receiverAccountId, string periodEnd)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@receiverAccountId", receiverAccountId, DbType.Int64);
                parameters.Add("@periodEnd", periodEnd, DbType.String);

                return await c.QueryAsync<AccountTransfer>(
                    sql: "[employer_financial].[GetAccountTransfersByPeriodEnd]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }

        private static DataTable CreateTransferDataTable(IEnumerable<AccountTransfer> transfers)
        {
            var table = new DataTable();

            table.Columns.AddRange(new[]
            {
                new DataColumn("SenderAccountId", typeof(long)),
                new DataColumn("SenderAccountName", typeof(string)),
                new DataColumn("ReceiverAccountId", typeof(long)),
                new DataColumn("ReceiverAccountName", typeof(string)),
                new DataColumn("ApprenticeshipId", typeof(long)),
                new DataColumn("CourseName", typeof(string)),
                new DataColumn("Amount", typeof(decimal)),
                new DataColumn("PeriodEnd", typeof(string)),
                new DataColumn("Type", typeof(short)),
                new DataColumn("TransferDateDate", typeof(DateTime)),
            });

            foreach (var transfer in transfers)
            {
                table.Rows.Add(
                    transfer.SenderAccountId,
                    transfer.SenderAccountName,
                    transfer.ReceiverAccountId,
                    transfer.ReceiverAccountName,
                    transfer.ApprenticeshipId,
                    transfer.CourseName,
                    transfer.Amount,
                    transfer.PeriodEnd,
                    transfer.Type,
                    transfer.TransferDate);
            }

            table.AcceptChanges();

            return table;
        }
    }
}
