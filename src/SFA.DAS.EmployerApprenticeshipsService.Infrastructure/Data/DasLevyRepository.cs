using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Entities.Transaction;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class DasLevyRepository : BaseRepository, IDasLevyRepository
    {
        private readonly IMapper _mapper;

        public DasLevyRepository(LevyDeclarationProviderConfiguration configuration, IMapper mapper)
            : base(configuration)
        {
            _mapper = mapper;
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

                return await c.QueryAsync<TransactionEntity>(
                    sql: "[levy].[GetTransactionLines_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
            
            return MapTransactions(result);
        }

        public async Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new AccountIdUserTableParam(accountIds);
                
                return await c.QueryAsync<AccountBalance>(
                 "[levy].[GetAccountBalance_ByAccountIds]",
                 parameters,
                 commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task CreateNewPeriodEnd(PeriodEnd periodEnd)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PeriodEndId", periodEnd.Id, DbType.String);
                parameters.Add("@CalendarPeriodMonth", periodEnd.CalendarPeriod.Month, DbType.Int32);
                parameters.Add("@CalendarPeriodYear", periodEnd.CalendarPeriod.Year, DbType.Int32);
                parameters.Add("@AccountDataValidAt", periodEnd.ReferenceData.AccountDataValidAt, DbType.DateTime);
                parameters.Add("@CommitmentDataValidAt", periodEnd.ReferenceData.CommitmentDataValidAt, DbType.DateTime);
                parameters.Add("@CompletionDateTime", periodEnd.CompletionDateTime, DbType.DateTime);
                parameters.Add("@PaymentsForPeriod", periodEnd.Links.PaymentsForPeriod, DbType.String);
                
                return await c.ExecuteAsync(
                    sql: "[levy].[CreatePeriodEnd]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<PeriodEnd> GetLatestPeriodEnd()
        {
            var result = await WithConnection(async c => await c.QueryAsync<PeriodEnd>(
                "[levy].[GetLatestPeriodEnd]",
                null,
                commandType: CommandType.StoredProcedure));
            
            return result.SingleOrDefault();
        }

        public async Task<List<TransactionLine>> GetTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@fromDate", new DateTime(fromDate.Year,fromDate.Month,fromDate.Day), DbType.DateTime);
                parameters.Add("@toDate", new DateTime(toDate.Year, toDate.Month, toDate.Day,23,59,59), DbType.DateTime);

                return await c.QueryAsync<TransactionEntity>(
                    sql: "[levy].[GetTransactionDetail_ByDateRange]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return MapTransactions(result);
        }

        public async Task CreatePaymentData(Payment payment, long accountId, string periodEnd, string providerName)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PaymentId", Guid.Parse(payment.Id), DbType.Guid);
                parameters.Add("@Ukprn", payment.Ukprn, DbType.Int64);
                parameters.Add("@ProviderName", providerName, DbType.StringFixedLength,ParameterDirection.Input,250);
                parameters.Add("@Uln", payment.Uln, DbType.Int64);
                parameters.Add("@AccountId", accountId, DbType.Int64);
                parameters.Add("@ApprenticeshipId", payment.ApprenticeshipId, DbType.Int64);
                parameters.Add("@DeliveryPeriodMonth", payment.DeliveryPeriod.Month, DbType.Int32);
                parameters.Add("@DeliveryPeriodYear", payment.DeliveryPeriod.Year, DbType.Int32);
                parameters.Add("@CollectionPeriodId", payment.CollectionPeriod.Id, DbType.String);
                parameters.Add("@CollectionPeriodMonth", payment.CollectionPeriod.Month, DbType.Int32);
                parameters.Add("@CollectionPeriodYear", payment.CollectionPeriod.Year, DbType.Int32);
                parameters.Add("@EvidenceSubmittedOn", payment.EvidenceSubmittedOn, DbType.DateTime);
                parameters.Add("@EmployerAccountVersion", payment.EmployerAccountVersion, DbType.String);
                parameters.Add("@ApprenticeshipVersion", payment.ApprenticeshipVersion, DbType.String);
                parameters.Add("@FundingSource", payment.FundingSource, DbType.String);
                parameters.Add("@TransactionType", payment.TransactionType, DbType.String);
                parameters.Add("@Amount", payment.Amount, DbType.Decimal);
                parameters.Add("@PeriodEnd", periodEnd, DbType.String);
                parameters.Add("@StandardCode", payment.StandardCode, DbType.Int64);
                parameters.Add("@FrameworkCode", payment.FrameworkCode, DbType.Int32);
                parameters.Add("@ProgrammeType", payment.ProgrammeType, DbType.Int32);
                parameters.Add("@PathwayCode", payment.PathwayCode, DbType.Int32);

                return await c.ExecuteAsync(
                    sql: "[levy].[CreatePayment]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<Payment> GetPaymentData(Guid paymentId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@paymentId", paymentId, DbType.Guid);

                return await c.QueryAsync<Payment>(
                    sql: "[levy].[GetPaymentData_ById]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task ProcessPaymentData()
        {
            await WithConnection(async c => await c.ExecuteAsync(
                sql: "[levy].[ProcessPaymentDataTransactions]",
                param: null,
                commandType: CommandType.StoredProcedure));
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

