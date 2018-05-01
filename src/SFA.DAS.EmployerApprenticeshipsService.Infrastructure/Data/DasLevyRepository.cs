using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class DasLevyRepository : BaseRepository, IDasLevyRepository
    {
        private readonly LevyDeclarationProviderConfiguration _configuration;
        private readonly ILog _logger;


        public DasLevyRepository(LevyDeclarationProviderConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _configuration = configuration;
            _logger = logger;
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

        public async Task<IEnumerable<long>> GetEmployerDeclarationSubmissionIds(string empRef)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@empRef", empRef, DbType.String);

            var result = await WithConnection(async c => await c.QueryAsync<long>(
                sql: "[employer_financial].[GetLevyDeclarationSubmissionIdsByEmpRef]",
                param: parameters,
                commandType: CommandType.StoredProcedure));

            return result.ToList();
        }

        public async Task CreateEmployerDeclarations(IEnumerable<DasDeclaration> declarations, string empRef, long accountId)
        {
            using (var connection = new SqlConnection(_configuration.DatabaseConnectionString))
            {
                await connection.OpenAsync();

                using (var unitOfWork = new UnitOfWork(connection))
                {
                    try
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
                            if (dasDeclaration.InactiveFrom.HasValue &&
                                dasDeclaration.InactiveFrom != DateTime.MinValue)
                            {
                                parameters.Add("@InactiveFrom", dasDeclaration.InactiveFrom, DbType.DateTime);
                            }
                            if (dasDeclaration.InactiveTo.HasValue && dasDeclaration.InactiveTo != DateTime.MinValue)
                            {
                                parameters.Add("@InactiveTo", dasDeclaration.InactiveTo, DbType.DateTime);
                            }

                            parameters.Add("@EndOfYearAdjustment", dasDeclaration.EndOfYearAdjustment, DbType.Boolean);
                            parameters.Add("@EndOfYearAdjustmentAmount", dasDeclaration.EndOfYearAdjustmentAmount,
                                DbType.Decimal);

                            parameters.Add("@NoPaymentForPeriod", dasDeclaration.NoPaymentForPeriod, DbType.Boolean);

                            await unitOfWork.Execute("[employer_financial].[CreateDeclaration]", parameters,
                                CommandType.StoredProcedure);
                        }

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception)
                    {
                        unitOfWork.RollbackChanges();
                        throw;
                    }
                }
            }
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

        public async Task ProcessDeclarations(long accountId, string empRef)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AccountId", accountId, DbType.Int64);
            parameters.Add("@EmpRef", empRef, DbType.String);

            await WithConnection(async c => await c.ExecuteAsync(
                sql: "[employer_financial].[ProcessDeclarationsTransactions]",
                param: parameters,
                commandType: CommandType.StoredProcedure));
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

        public async Task CreateNewPeriodEnd(PeriodEnd periodEnd)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PeriodEndId", periodEnd.Id, DbType.String);
                parameters.Add("@CalendarPeriodMonth", periodEnd.CalendarPeriodMonth, DbType.Int32);
                parameters.Add("@CalendarPeriodYear", periodEnd.CalendarPeriodYear, DbType.Int32);
                parameters.Add("@AccountDataValidAt", periodEnd.AccountDataValidAt, DbType.DateTime);
                parameters.Add("@CommitmentDataValidAt", periodEnd.CommitmentDataValidAt, DbType.DateTime);
                parameters.Add("@CompletionDateTime", periodEnd.CompletionDateTime, DbType.DateTime);
                parameters.Add("@PaymentsForPeriod", periodEnd.PaymentsForPeriod, DbType.String);

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

        public async Task<IEnumerable<PeriodEnd>> GetAllPeriodEnds()
        {
            var result = await WithConnection(async c => await c.QueryAsync<PeriodEnd>(
                "[employer_financial].[GetAllPeriodEnds]",
                null,
                commandType: CommandType.StoredProcedure));

            return result;
        }

        public async Task CreatePaymentData(IEnumerable<PaymentDetails> payments)
        {
            using (var connection = new SqlConnection(_configuration.DatabaseConnectionString))
            {
                await connection.OpenAsync();

                using (var unitOfWork = new UnitOfWork(connection))
                {
                    try
                    {
                        foreach (var details in payments)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@PaymentId", Guid.Parse(details.Id), DbType.Guid);
                            parameters.Add("@Ukprn", details.Ukprn, DbType.Int64);
                            parameters.Add("@ProviderName", details.ProviderName, DbType.String);
                            parameters.Add("@Uln", details.Uln, DbType.Int64);
                            parameters.Add("@AccountId", details.EmployerAccountId, DbType.Int64);
                            parameters.Add("@ApprenticeshipId", details.ApprenticeshipId, DbType.Int64);
                            parameters.Add("@DeliveryPeriodMonth", details.DeliveryPeriodMonth, DbType.Int32);
                            parameters.Add("@DeliveryPeriodYear", details.DeliveryPeriodYear, DbType.Int32);
                            parameters.Add("@CollectionPeriodId", details.CollectionPeriodId, DbType.String);
                            parameters.Add("@CollectionPeriodMonth", details.CollectionPeriodMonth, DbType.Int32);
                            parameters.Add("@CollectionPeriodYear", details.CollectionPeriodYear, DbType.Int32);
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
                            parameters.Add("@PathwayName", details.PathwayName, DbType.String);
                            parameters.Add("@CourseName", details.CourseName, DbType.String);
                            parameters.Add("@ApprenticeName", details.ApprenticeName, DbType.String);
                            parameters.Add("@ApprenticeNINumber", details.ApprenticeNINumber, DbType.String);
                            parameters.Add("@ApprenticeshipCourseLevel", details.CourseLevel, DbType.Int32);
                            parameters.Add("@ApprenticeshipCourseStartDate", details.CourseStartDate, DbType.DateTime);

                            await unitOfWork.Execute("[employer_financial].[CreatePayment]", parameters,
                                CommandType.StoredProcedure);
                        }

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "CreatePaymentData failed: " + ex.Message);
                        unitOfWork.RollbackChanges();
                        throw;
                    }
                }
            }
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

        public async Task<IEnumerable<Guid>> GetAccountPaymentIds(long accountId)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);

                return await c.QueryAsync<Guid>(
                    sql: "[employer_financial].[GetAccountPaymentIds]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result.ToArray();
        }

        public async Task ProcessPaymentData(long accountId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@accountId", accountId, DbType.Int64);

            await WithConnection(async c => await c.ExecuteAsync(
                sql: "[employer_financial].[ProcessPaymentDataTransactions]",
                param: parameters,
                commandType: CommandType.StoredProcedure));
        }

        public async Task<IEnumerable<Payment>> GetAccountPaymentsByPeriodEnd(long accountId, string periodEnd)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@periodEnd", periodEnd, DbType.String);

                return await c.QueryAsync<Payment>(
                    sql: "[employer_financial].[GetAccountPaymentsByPeriodEnd]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(long accountId, string empRef)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@empRef", empRef, DbType.String);

                return await c.QueryAsync<DasEnglishFraction>(
                    sql: "[employer_financial].[GetEnglishFraction_ByEmpRef]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
            });

            return result;
        }
    }
}

