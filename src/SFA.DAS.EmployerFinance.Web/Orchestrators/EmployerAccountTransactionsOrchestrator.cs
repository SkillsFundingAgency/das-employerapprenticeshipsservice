using MediatR;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.FindAccountProviderPayments;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.Exceptions;
using SFA.DAS.NLog.Logger;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Web.Orchestrators
{
    public class EmployerAccountTransactionsOrchestrator
    {
        //private readonly ICurrentDateTime _currentTime;
        private readonly ILog _logger;
        private readonly IMediator _mediator;

        protected EmployerAccountTransactionsOrchestrator()
        {

        }

        public EmployerAccountTransactionsOrchestrator(IMediator mediator/*, ICurrentDateTime currentTime*/, ILog logger)
        {
            _mediator = mediator;
            //_currentTime = currentTime;
            _logger = logger;
        }

        public async Task<OrchestratorResponse<ProviderPaymentsSummaryViewModel>> GetProviderPaymentSummary(
            string hashedId, long ukprn, DateTime fromDate, DateTime toDate, string externalUserId)
        {
            try
            {
                var data = await _mediator.SendAsync(new FindAccountProviderPaymentsQuery
                {
                    HashedAccountId = hashedId,
                    UkPrn = ukprn,
                    FromDate = fromDate,
                    ToDate = toDate,
                    ExternalUserId = externalUserId,
                });

                var courseGroups =
                    data.Transactions.GroupBy(x => new { x.CourseName, x.CourseLevel, x.PathwayName, x.CourseStartDate });

                var coursePaymentSummaries = courseGroups.Select(x =>
                {
                    var levyPayments = x.Where(p => p.TransactionType == TransactionItemType.Payment).ToList();

                    return new CoursePaymentSummaryViewModel
                    {
                        CourseName = x.Key.CourseName,
                        CourseLevel = x.Key.CourseLevel,
                        PathwayName = x.Key.PathwayName,
                        PathwayCode = levyPayments.Max(p => p.PathwayCode),
                        CourseStartDate = x.Key.CourseStartDate,
                        LevyPaymentAmount = levyPayments.Sum(p => p.LineAmount),
                        EmployerCoInvestmentAmount = levyPayments.Sum(p => p.EmployerCoInvestmentAmount),
                        SFACoInvestmentAmount = levyPayments.Sum(p => p.SfaCoInvestmentAmount)
                    };
                }).ToList();

                return new OrchestratorResponse<ProviderPaymentsSummaryViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new ProviderPaymentsSummaryViewModel
                    {
                        HashedAccountId = hashedId,
                        UkPrn = ukprn,
                        ProviderName = data.ProviderName,
                        PaymentDate = data.DateCreated,
                        FromDate = fromDate,
                        ToDate = toDate,
                        CoursePayments = coursePaymentSummaries,
                        LevyPaymentsTotal = coursePaymentSummaries.Sum(p => p.LevyPaymentAmount),
                        SFACoInvestmentsTotal = coursePaymentSummaries.Sum(p => p.SFACoInvestmentAmount),
                        EmployerCoInvestmentsTotal = coursePaymentSummaries.Sum(p => p.EmployerCoInvestmentAmount),
                        PaymentsTotal = coursePaymentSummaries.Sum(p => p.TotalAmount)
                    }
                };
            }
            catch (NotFoundException e)
            {
                return new OrchestratorResponse<ProviderPaymentsSummaryViewModel>
                {
                    Status = HttpStatusCode.NotFound,
                    Exception = e
                };
            }
            catch (InvalidRequestException e)
            {
                return new OrchestratorResponse<ProviderPaymentsSummaryViewModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    Exception = e
                };
            }
            catch (UnauthorizedAccessException e)
            {
                return new OrchestratorResponse<ProviderPaymentsSummaryViewModel>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = e
                };
            }
        }

        public Task<string> GetCoursePaymentSummary(string hashedAccountId, long ukprn, string courseName, int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate, string getClaimValue)
        {
            // Dummy method for replacement by future merge
            throw new NotImplementedException();
        }
    }
}