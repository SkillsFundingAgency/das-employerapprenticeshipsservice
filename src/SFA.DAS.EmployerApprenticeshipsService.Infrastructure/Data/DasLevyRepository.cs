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
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Entities.Transaction;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.Payments.Events.Api.Types;
using Payment = SFA.DAS.Payments.Events.Api.Types.Payment;

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
                    sql: "SELECT LevyDueYtd, SubmissionId AS Id, SubmissionDate AS [Date] FROM [employer_financial].[LevyDeclaration] WHERE empRef = @EmpRef and SubmissionId = @Id;",
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
                parameters.Add("@SubmissionDate", dasDeclaration.SubmissionDate, DbType.DateTime);
                parameters.Add("@SubmissionId", dasDeclaration.Id, DbType.String);
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
                
                parameters.Add("@EndOfYearAdjustment", dasDeclaration.EndOfYearAdjustment,DbType.Boolean);
                parameters.Add("@EndOfYearAdjustmentAmount", dasDeclaration.EndOfYearAdjustmentAmount, DbType.Decimal);
                
                return await c.ExecuteAsync(
                    sql: "[employer_financial].[CreateDeclaration]",
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
                    sql: "[employer_financial].[GetLevyDeclarations_ByAccountId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        public async Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@payrollYear", payrollYear, DbType.String);
                parameters.Add("@payrollMonth", payrollMonth, DbType.Int16);

                return await c.QueryAsync<LevyDeclarationView>(
                    sql: "[employer_financial].[GetLevyDeclarations_ByAccountPayrollMonthPayrollYear]",
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
                    sql: "[employer_financial].[GetLastLevyDeclarations_ByEmpRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task<DasDeclaration> GetSubmissionByEmprefPayrollYearAndMonth(string empRef, string payrollYear, short payrollMonth)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", empRef, DbType.String);
                parameters.Add("@payrollYear", payrollYear, DbType.String);
                parameters.Add("@payrollMonth", payrollMonth, DbType.Int32);

                return await c.QueryAsync<DasDeclaration>(
                    sql: "[employer_financial].[GetLevyDeclaration_ByEmpRefPayrollMonthPayrollYear]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task ProcessDeclarations()
        {
            await WithConnection(async c => await c.ExecuteAsync(
                sql: "[employer_financial].[ProcessDeclarationsTransactions]",
                param: null,
                commandType: CommandType.StoredProcedure));
        }

        public async Task<List<TransactionLine>> GetTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate)
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

        public async Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new AccountIdUserTableParam(accountIds);
                
                return await c.QueryAsync<AccountBalance>(
                 "[employer_financial].[GetAccountBalance_ByAccountIds]",
                 parameters,
                 commandType: CommandType.StoredProcedure);
            });

            return result.ToList();
        }

        //TODO refactor not to use PeriodEnd type here
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
                    sql: "[employer_financial].[CreatePeriodEnd]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<PeriodEnd> GetLatestPeriodEnd()
        {
            var result = await WithConnection(async c => await c.QueryAsync<PeriodEnd>(
                "[employer_financial].[GetLatestPeriodEnd]",
                null,
                commandType: CommandType.StoredProcedure));
            
            return result.SingleOrDefault();
        }

        public async Task<List<TransactionLine>> GetTransactionDetailsByDateRange(long accountId, DateTime fromDate, DateTime toDate)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@fromDate", new DateTime(fromDate.Year,fromDate.Month,fromDate.Day), DbType.DateTime);
                parameters.Add("@toDate", new DateTime(toDate.Year, toDate.Month, toDate.Day,23,59,59), DbType.DateTime);

                return await c.QueryAsync<TransactionEntity>(
                    sql: "[employer_financial].[GetTransactionDetail_ByDateRange]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return MapTransactions(result);
        }
        
        public async Task CreatePaymentData(PaymentDetails details)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PaymentId", Guid.Parse(details.Id), DbType.Guid);
                parameters.Add("@Ukprn", details.Ukprn, DbType.Int64);
                parameters.Add("@ProviderName", details.ProviderName, DbType.String);
                parameters.Add("@Uln", details.Uln, DbType.Int64);
                parameters.Add("@AccountId", details.EmployerAccountId, DbType.Int64);
                parameters.Add("@ApprenticeshipId", details.ApprenticeshipId, DbType.Int64);
                parameters.Add("@DeliveryPeriodMonth", details.DeliveryPeriod.Month, DbType.Int32);
                parameters.Add("@DeliveryPeriodYear", details.DeliveryPeriod.Year, DbType.Int32);
                parameters.Add("@CollectionPeriodId", details.CollectionPeriod.Id, DbType.String);
                parameters.Add("@CollectionPeriodMonth", details.CollectionPeriod.Month, DbType.Int32);
                parameters.Add("@CollectionPeriodYear", details.CollectionPeriod.Year, DbType.Int32);
                parameters.Add("@EvidenceSubmittedOn", details.EvidenceSubmittedOn, DbType.DateTime);
                parameters.Add("@EmployerAccountVersion", details.EmployerAccountVersion, DbType.String);
                parameters.Add("@ApprenticeshipVersion", details.ApprenticeshipVersion, DbType.String);
                parameters.Add("@FundingSource", details.FundingSource, DbType.String);
                parameters.Add("@TransactionType", details.TransactionType, DbType.String);
                parameters.Add("@Amount", details.Amount, DbType.Decimal);
                parameters.Add("@PeriodEnd", details.PeriodEnd, DbType.String);
                parameters.Add("@StandardCode", details.StandardCode, DbType.Int64);
                parameters.Add("@FrameworkCode", details.FrameworkCode, DbType.Int32);
                parameters.Add("@ProgrammeType", details.ProgrammeType, DbType.Int32);
                parameters.Add("@PathwayCode", details.PathwayCode, DbType.Int32);
                parameters.Add("@CourseName", details.CourseName, DbType.String);
                parameters.Add("@ApprenticeName", details.ApprenticeName, DbType.String);
                parameters.Add("@ApprenticeNINumber", details.ApprenticeNINumber, DbType.String);
                parameters.Add("@ApprenticeshipCourseLevel", details.CourseLevel, DbType.Int32);
                parameters.Add("@ApprenticeshipCourseStartDate", details.CourseStartDate, DbType.DateTime);

                return await c.ExecuteAsync(
                    sql: "[employer_financial].[CreatePayment]",
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
                    sql: "[employer_financial].[GetPaymentData_ById]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.SingleOrDefault();
        }

        public async Task ProcessPaymentData()
        {
            await WithConnection(async c => await c.ExecuteAsync(
                sql: "[employer_financial].[ProcessPaymentDataTransactions]",
                param: null,
                commandType: CommandType.StoredProcedure));
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "[employer_financial].[GetEnglishFraction_ByEmpRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;
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
                    case TransactionItemType.SFACoInvestment:
                    case TransactionItemType.EmployerCoInvestment:
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

