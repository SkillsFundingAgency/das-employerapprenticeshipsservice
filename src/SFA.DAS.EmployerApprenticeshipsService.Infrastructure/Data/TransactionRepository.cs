using AutoMapper;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.ExpiredFunds;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        private readonly IMapper _mapper;
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public TransactionRepository(LevyDeclarationProviderConfiguration configuration, IMapper mapper, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<List<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@fromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day), DbType.DateTime);
            parameters.Add("@toDate", new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59), DbType.DateTime);

            var result = await _db.Value.Database.Connection.QueryAsync<TransactionEntity>(
                sql: "[employer_financial].[GetTransactionLines_ByAccountId]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return MapTransactions(result);
        }
        
        public Task<int> GetPreviousTransactionsCount(long accountId, DateTime fromDate)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@fromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day), DbType.DateTime);

            return _db.Value.Database.Connection.ExecuteScalarAsync<int>(
                sql: "[employer_financial].[GetPreviousTransactionsCount]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<List<TransactionSummary>> GetAccountTransactionSummary(long accountId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@AccountId", accountId, DbType.Int64);

            var result = await _db.Value.Database.Connection.QueryAsync<TransactionSummary>(
                sql: "[employer_financial].[GetTransactionSummary_ByAccountId]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
        
        public async Task<string> GetProviderName(int ukprn, long accountId, string periodEnd)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@ukprn", ukprn, DbType.Int64);
            parameters.Add("@accountId", accountId, DbType.Int64);
            parameters.Add("@periodEnd", periodEnd, DbType.String);

            return await _db.Value.Database.Connection.ExecuteScalarAsync<string>(
                sql: "[employer_financial].[GetProviderName]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
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

                    case TransactionItemType.ExpiredFund:
                        transactions.Add(_mapper.Map<ExpiredFundTransactionLine>(entity));
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