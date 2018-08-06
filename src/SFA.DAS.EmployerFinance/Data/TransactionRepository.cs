using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerFinance.Data
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        private readonly IMapper _mapper;
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public TransactionRepository(EmployerFinanceConfiguration configuration, IMapper mapper, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _mapper = mapper;
            _db = db;
        }

        public Task CreateTransferTransactions(IEnumerable<TransferTransactionLine> transactions)
        {
            var transactionTable = CreateTransferTransactionDataTable(transactions);
            var parameters = new DynamicParameters();

            parameters.Add("@transferTransactions", transactionTable.AsTableValuedParameter("[employer_financial].[TransferTransactionsTable]"));

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[CreateAccountTransferTransactions]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        private static DataTable CreateTransferTransactionDataTable(IEnumerable<TransferTransactionLine> transactions)
        {
            var table = new DataTable();

            table.Columns.AddRange(new[]
            {
                new DataColumn("AccountId", typeof(long)),
                new DataColumn("SenderAccountId", typeof(long)),
                new DataColumn("SenderAccountName", typeof(string)),
                new DataColumn("ReceiverAccountId", typeof(long)),
                new DataColumn("ReceiverAccountName", typeof(string)),
                new DataColumn("PeriodEnd", typeof(string)),
                new DataColumn("Amount", typeof(decimal)),
                new DataColumn("TransactionType", typeof(short)),
                new DataColumn("TransactionDate", typeof(DateTime)),
            });

            foreach (var transaction in transactions)
            {
                table.Rows.Add(
                    transaction.AccountId,
                    transaction.SenderAccountId,
                    transaction.SenderAccountName,
                    transaction.ReceiverAccountId,
                    transaction.ReceiverAccountName,
                    transaction.PeriodEnd,
                    transaction.Amount,
                    (short)TransactionItemType.Transfer,
                    transaction.TransactionDate);
            }

            table.AcceptChanges();

            return table;
        }
    }
}
