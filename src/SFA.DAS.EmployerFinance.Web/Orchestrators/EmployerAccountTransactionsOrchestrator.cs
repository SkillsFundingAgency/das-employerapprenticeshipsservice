using MediatR;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.FindAccountCoursePayments;
using SFA.DAS.EmployerFinance.Queries.FindAccountProviderPayments;
using SFA.DAS.EmployerFinance.Queries.FindEmployerAccountLevyDeclarationTransactions;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccount;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EmployerFinance.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Web.Orchestrators
{
    public class EmployerAccountTransactionsOrchestrator
    {
        private readonly ICurrentDateTime _currentTime;
        private readonly ILog _logger;
        private readonly IMediator _mediator;

        protected EmployerAccountTransactionsOrchestrator()
        {

        }

        public EmployerAccountTransactionsOrchestrator(IMediator mediator, ICurrentDateTime currentTime, ILog logger)
        {
            _mediator = mediator;
            _currentTime = currentTime;
            _logger = logger;
        }

        public async Task<OrchestratorResponse<PaymentTransactionViewModel>> FindAccountPaymentTransactions(
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
                    ExternalUserId = externalUserId
                });

                var courseGroups = data.Transactions.GroupBy(x => new { x.CourseName, x.CourseLevel, x.CourseStartDate });

                var coursePaymentGroups = courseGroups.Select(x => new ApprenticeshipPaymentGroup
                {
                    ApprenticeCourseName = x.Key.CourseName,
                    CourseLevel = x.Key.CourseLevel,
                    CourseStartDate = x.Key.CourseStartDate,
                    Payments = x.ToList()
                }).ToList();


                return new OrchestratorResponse<PaymentTransactionViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new PaymentTransactionViewModel
                    {
                        ProviderName = data.ProviderName,
                        TransactionDate = data.TransactionDate,
                        Amount = data.Total,
                        SubTransactions = data.Transactions,
                        CoursePaymentGroups = coursePaymentGroups
                    }
                };
            }
            catch (NotFoundException e)
            {
                return new OrchestratorResponse<PaymentTransactionViewModel>
                {
                    Status = HttpStatusCode.NotFound,
                    Exception = e
                };
            }
            catch (InvalidRequestException e)
            {
                return new OrchestratorResponse<PaymentTransactionViewModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    Exception = e
                };
            }
            catch (UnauthorizedAccessException e)
            {
                return new OrchestratorResponse<PaymentTransactionViewModel>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = e
                };
            }
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

        public virtual async Task<OrchestratorResponse<CoursePaymentDetailsViewModel>> GetCoursePaymentSummary(
            string hashedAccountId, long ukprn, string courseName, int? courseLevel, int? pathwayCode,
            DateTime fromDate, DateTime toDate, string externalUserId)
        {
            try
            {
                var data = await _mediator.SendAsync(new FindAccountCoursePaymentsQuery
                {
                    HashedAccountId = hashedAccountId,
                    UkPrn = ukprn,
                    CourseName = courseName,
                    CourseLevel = courseLevel,
                    PathwayCode = pathwayCode,
                    FromDate = fromDate,
                    ToDate = toDate,
                    ExternalUserId = externalUserId
                });

                var apprenticePaymentGroups = data.Transactions.GroupBy(x => new { x.ApprenticeName, x.ApprenticeNINumber });

                var paymentSummaries = apprenticePaymentGroups.Select(pg =>
                {
                    var payments = pg.Where(x => x.TransactionType == TransactionItemType.Payment).ToList();

                    return new AprrenticeshipPaymentSummaryViewModel
                    {
                        ApprenticeName = pg.Key.ApprenticeName,
                        LevyPaymentAmount = payments.Sum(t => t.LineAmount),
                        SFACoInvestmentAmount = payments.Sum(p => p.SfaCoInvestmentAmount),
                        EmployerCoInvestmentAmount = payments.Sum(p => p.EmployerCoInvestmentAmount)
                    };
                });

                var apprenticePayments = paymentSummaries.ToList();

                return new OrchestratorResponse<CoursePaymentDetailsViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new CoursePaymentDetailsViewModel
                    {
                        ProviderName = data.ProviderName,
                        CourseName = data.CourseName,
                        CourseLevel = data.CourseLevel,
                        PathwayName = data.PathwayName,
                        PaymentDate = data.DateCreated,
                        LevyPaymentsTotal = apprenticePayments.Sum(p => p.LevyPaymentAmount),
                        SFACoInvestmentTotal = apprenticePayments.Sum(p => p.SFACoInvestmentAmount),
                        EmployerCoInvestmentTotal = apprenticePayments.Sum(p => p.EmployerCoInvestmentAmount),
                        ApprenticePayments = apprenticePayments,
                        HashedAccountId = hashedAccountId
                    }
                };
            }
            catch (NotFoundException e)
            {
                return new OrchestratorResponse<CoursePaymentDetailsViewModel>
                {
                    Status = HttpStatusCode.NotFound,
                    Exception = e
                };
            }
            catch (InvalidRequestException e)
            {
                return new OrchestratorResponse<CoursePaymentDetailsViewModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    Exception = e
                };
            }
            catch (UnauthorizedAccessException e)
            {
                return new OrchestratorResponse<CoursePaymentDetailsViewModel>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = e
                };
            }
        }

        public virtual async Task<OrchestratorResponse<TransactionViewResultViewModel>> GetAccountTransactions(
            string hashedId, int year, int month, string externalUserId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = hashedId,
                UserId = externalUserId
            });

            if (employerAccountResult.Account == null)
            {
                return new OrchestratorResponse<TransactionViewResultViewModel>
                {
                    Data = new TransactionViewResultViewModel(_currentTime.Now)
                };
            }

            var aggregratedTransactions =
                await
                    _mediator.SendAsync(new GetEmployerAccountTransactionsQuery
                    {
                        ExternalUserId = externalUserId,
                        Year = year,
                        Month = month,
                        HashedAccountId = hashedId
                    });

            var viewModel = BuildTransactionViewModel(aggregratedTransactions.Data, year, month);

            return new OrchestratorResponse<TransactionViewResultViewModel>
            {
                Data = new TransactionViewResultViewModel(_currentTime.Now)
                {
                    Account = employerAccountResult.Account,
                    Model = viewModel,
                    Month = aggregratedTransactions.Month,
                    Year = aggregratedTransactions.Year,
                    AccountHasPreviousTransactions = aggregratedTransactions.AccountHasPreviousTransactions
                }
            };
        }


        public virtual async Task<OrchestratorResponse<FinanceDashboardViewModel>> GetFinanceDashboardViewModel(
            string hashedId, int year, int month, string externalUserId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = hashedId,
                UserId = externalUserId
            });

            if (employerAccountResult.Account == null)
            {
                return new OrchestratorResponse<FinanceDashboardViewModel>
                {
                    Data = new FinanceDashboardViewModel()
                };
            }

            employerAccountResult.Account.HashedId = hashedId;

            var data =
                await
                    _mediator.SendAsync(new GetEmployerAccountTransactionsQuery
                    {
                        ExternalUserId = externalUserId,
                        Year = year,
                        Month = month,
                        HashedAccountId = hashedId
                    });


            return new OrchestratorResponse<FinanceDashboardViewModel>
            {
                Data = new FinanceDashboardViewModel
                {
                    Account = employerAccountResult.Account,
                    CurrentLevyFunds = data.Data.Balance
                }
            };
        }

        private TransactionViewModel BuildTransactionViewModel(AggregationData aggregationData, int year, int month)
        {
            var viewModel = new TransactionViewModel
            {
                Data = new AggregationData
                {
                    AccountId = aggregationData.AccountId,
                    HashedAccountId = aggregationData.HashedAccountId,
                },
                CurrentBalance = aggregationData.Balance
            };

            SetTransactionLines(viewModel, aggregationData);
            return viewModel;
        }

        private void SetTransactionLines(TransactionViewModel viewModel, AggregationData aggregatedTransactionData)
        {
            var aggregatedLevyTransactions = aggregatedTransactionData.TransactionLines
                .Where(t => t.TransactionType == TransactionItemType.Declaration)
                .GroupBy(t => t.DateCreated.Date)
                .Select(grp =>
                {
                    var firstLevyTransactionInDay = grp.First();
                    return new TransactionLine
                    {
                        AccountId = firstLevyTransactionInDay.AccountId,
                        DateCreated = firstLevyTransactionInDay.DateCreated,
                        Amount = grp.Sum(ltl => ltl.Amount),
                        TransactionType = TransactionItemType.Declaration,
                        Description = firstLevyTransactionInDay.Description,
                        PayrollDate = firstLevyTransactionInDay.PayrollDate,
                        PayrollMonth = firstLevyTransactionInDay.PayrollMonth,
                        PayrollYear = firstLevyTransactionInDay.PayrollYear
                    };
                });

            var newTransactionLines = aggregatedTransactionData.TransactionLines
                .Where(t => t.TransactionType != TransactionItemType.Declaration)
                .Union(aggregatedLevyTransactions)
                .ToArray();

            viewModel.Data.TransactionLines = newTransactionLines;
        }

        public async Task<OrchestratorResponse<TransactionLineViewModel<LevyDeclarationTransactionLine>>>
            FindAccountLevyDeclarationTransactions(
                string hashedId, DateTime fromDate, DateTime toDate, string externalUserId)
        {
            var data = await _mediator.SendAsync(new FindEmployerAccountLevyDeclarationTransactionsQuery
            {
                HashedAccountId = hashedId,
                FromDate = fromDate,
                ToDate = toDate,
                ExternalUserId = externalUserId
            });

            foreach (var transaction in data.Transactions)
            {
                var payeSchemeData = await _mediator.SendAsync(new GetPayeSchemeByRefQuery
                {
                    HashedAccountId = hashedId,
                    Ref = transaction.EmpRef
                });

                transaction.PayeSchemeName = payeSchemeData?.PayeScheme?.Name ?? string.Empty;
            }

            if (data.Transactions.Count == 0)
            {
                return new OrchestratorResponse<TransactionLineViewModel<LevyDeclarationTransactionLine>>
                {
                    Status = HttpStatusCode.NotFound
                };
            }
            return new OrchestratorResponse<TransactionLineViewModel<LevyDeclarationTransactionLine>>
            {
                Status = HttpStatusCode.OK,
                Data = new TransactionLineViewModel<LevyDeclarationTransactionLine>
                {
                    Amount = data.Total,
                    SubTransactions = data.Transactions,
                    TransactionDate = data.Transactions.First().DateCreated
                }
            };
        }
    }
}