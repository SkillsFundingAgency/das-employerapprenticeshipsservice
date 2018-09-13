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
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public TransferRepository(LevyDeclarationProviderConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _allowancePercentage = configuration.TransferAllowancePercentage;
            _db = db;
        }

        public Task CreateAccountTransfers(IEnumerable<AccountTransfer> transfers)
        {
            var accountTransfers = transfers as AccountTransfer[] ?? transfers.ToArray();
            var transferDataTable = CreateTransferDataTable(accountTransfers);
            var parameters = new DynamicParameters();

            parameters.Add("@transfers", transferDataTable.AsTableValuedParameter("[employer_financial].[AccountTransferTable]"));

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[CreateAccountTransfers]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        //public async Task<decimal> GetTransferAllowance(long accountId)
        //{
        //    var result = await WithConnection(async c =>
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@accountId", accountId, DbType.Int64);
        //        parameters.Add("@allowancePercentage", _allowancePercentage, DbType.Single);

        //        return await c.QuerySingleOrDefaultAsync<decimal?>(
        //            sql: "[employer_financial].[GetAccountTransferAllowance]",
        //            param: parameters,
        //            commandType: CommandType.StoredProcedure);
        //    });

        //    return result ?? 0;
        //}

        public Task<AccountTransferDetails> GetTransferPaymentDetails(AccountTransfer transfer)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@receiverAccountId", transfer.ReceiverAccountId, DbType.Int64);
            parameters.Add("@periodEnd", transfer.PeriodEnd, DbType.String);
            parameters.Add("@apprenticeshipId", transfer.CommitmentId, DbType.Int64);

            return _db.Value.Database.Connection.QuerySingleOrDefaultAsync<AccountTransferDetails>(
                sql: "[employer_financial].[GetTransferPaymentDetails]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public Task<IEnumerable<AccountTransfer>> GetReceiverAccountTransfersByPeriodEnd(long receiverAccountId, string periodEnd)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@receiverAccountId", receiverAccountId, DbType.Int64);
            parameters.Add("@periodEnd", periodEnd, DbType.String);

            return _db.Value.Database.Connection.QueryAsync<AccountTransfer>(
                sql: "[employer_financial].[GetAccountTransfersByPeriodEnd]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
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
                new DataColumn("CourseLevel", typeof(int)),
                new DataColumn("Amount", typeof(decimal)),
                new DataColumn("PeriodEnd", typeof(string)),
                new DataColumn("Type", typeof(string)),
                new DataColumn("RequiredPaymentId", typeof(Guid)),
            });

            foreach (var transfer in transfers)
            {
                table.Rows.Add(
                    transfer.SenderAccountId,
                    transfer.SenderAccountName,
                    transfer.ReceiverAccountId,
                    transfer.ReceiverAccountName,
                    transfer.CommitmentId,
                    transfer.CourseName,
                    transfer.CourseLevel,
                    transfer.Amount,
                    transfer.PeriodEnd,
                    transfer.Type,
                    transfer.RequiredPaymentId);
            }

            table.AcceptChanges();

            return table;
        }
    }
}