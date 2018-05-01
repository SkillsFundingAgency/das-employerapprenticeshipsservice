using MediatR;
using SFA.DAS.EAS.Application.Queries.FindAccountCoursePayments;
using SFA.DAS.EAS.Application.Queries.FindAccountProviderPayments;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountLevyDeclarationTransactions;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.Edm.Library.Values;
using SFA.DAS.EAS.Application.Exceptions;

namespace SFA.DAS.EAS.Web.Orchestrators
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

        public async Task<OrchestratorResponse<TransactionLineViewModel<LevyDeclarationTransactionLine>>>
            FindAccountLevyDeclarationTransactions(
                string hashedId, DateTime fromDate, DateTime toDate, Guid externalUserId)
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

        public async Task<OrchestratorResponse<ProviderPaymentsSummaryViewModel>> GetProviderPaymentSummary(
            string hashedId, long ukprn, DateTime fromDate, DateTime toDate, Guid externalUserId)
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

                if (data == null)
                    return null;

                var courseGroups = data.Transactions.GroupBy(x => new { x.CourseName, x.CourseLevel, x.PathwayName, x.CourseStartDate });

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

        public async Task<OrchestratorResponse<PaymentTransactionViewModel>> FindAccountPaymentTransactions(
            string hashedId, long ukprn, DateTime fromDate, DateTime toDate, Guid externalUserId)
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

                if (data == null)
                    return null;

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

        public virtual async Task<OrchestratorResponse<FinanceDashboardViewModel>> GetFinanceDashboardViewModel(
            string hashedId, int year, int month, Guid externalUserId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = hashedId,
                ExternalUserId = externalUserId
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
            var latestLineItem = data?.Data?.TransactionLines?.FirstOrDefault();

            var currentBalance = latestLineItem?.Balance ?? 0;

            return new OrchestratorResponse<FinanceDashboardViewModel>
            {
                Data = new FinanceDashboardViewModel
                {
                    Account = employerAccountResult.Account,
                    CurrentLevyFunds = currentBalance
                }
            };
        }


        public virtual async Task<OrchestratorResponse<TransactionViewResultViewModel>> GetAccountTransactions(string hashedId, int year, int month, Guid externalUserId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = hashedId,
                ExternalUserId = externalUserId
            });

            if (employerAccountResult.Account == null)
            {
                return new OrchestratorResponse<TransactionViewResultViewModel> { Data = new TransactionViewResultViewModel(_currentTime.Now) };
            }

            var data =
                await
                    _mediator.SendAsync(new GetEmployerAccountTransactionsQuery
                    {
                        ExternalUserId = externalUserId,
                        Year = year,
                        Month = month,
                        HashedAccountId = hashedId
                    });
            var latestLineItem = data.Data.TransactionLines.FirstOrDefault();
            decimal currentBalance;
            DateTime currentBalanceCalcultedOn;

            if (latestLineItem != null)
            {
                currentBalance = latestLineItem.Balance;
                currentBalanceCalcultedOn = latestLineItem.TransactionDate;
            }
            else
            {
                currentBalance = 0;
                currentBalanceCalcultedOn = DateTime.Today;
            }

            data.Data.TransactionLines = AggregateLevyTransactions(data.Data.TransactionLines);

            return new OrchestratorResponse<TransactionViewResultViewModel>
            {
                Data = new TransactionViewResultViewModel(_currentTime.Now)
                {
                    Account = employerAccountResult.Account,
                    Model = new TransactionViewModel
                    {
                        CurrentBalance = currentBalance,
                        CurrentBalanceCalcultedOn = currentBalanceCalcultedOn,
                        Data = data.Data
                    },
                    Month = data.Month,
                    Year = data.Year,
                    AccountHasPreviousTransactions = data.AccountHasPreviousTransactions
                }
            };
        }


        public virtual async Task<OrchestratorResponse<CoursePaymentDetailsViewModel>> GetCoursePaymentSummary(
            string hashedAccountId, long ukprn, string courseName, int courseLevel, int? pathwayCode,
            DateTime fromDate, DateTime toDate, Guid externalUserId)
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

                if (data == null)
                    return null;

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
                        ApprenticePayments = apprenticePayments
                    }
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

        private static ICollection<TransactionLine> AggregateLevyTransactions(ICollection<TransactionLine> transactions)
        {
            if (transactions == null || transactions.Count == 0)
                return new List<TransactionLine>();

            if (!transactions.HasAtLeast(2, t => t.TransactionType == TransactionItemType.Declaration))
                return transactions;

            var levyTransactions = transactions.Where(t => t.TransactionType == TransactionItemType.Declaration)
                                               .ToList();

            var aggregatedTransactions = transactions.Except(levyTransactions).ToList();

            var levyTransaction = GetAggregatedLevyTransaction(levyTransactions);

            aggregatedTransactions.Add(levyTransaction);


            return aggregatedTransactions;
        }

        private static LevyDeclarationTransactionLine GetAggregatedLevyTransaction(List<TransactionLine> levyTransactions)
        {
            var maxDateCreated = DateTime.MinValue;
            var totalAmount = 0m;
            var maxTransactionDate = DateTime.MaxValue;

            foreach (var transaction in levyTransactions)
            {
                if (transaction.DateCreated > maxDateCreated)
                {
                    maxDateCreated = transaction.DateCreated;
                }

                if (transaction.TransactionDate > maxTransactionDate)
                {
                    maxTransactionDate = transaction.TransactionDate;
                }

                totalAmount += transaction.Amount;
            }

            //We haven't populated all the values here as the view doesn't support them yet
            var levyTransaction = new LevyDeclarationTransactionLine
            {
                Description = levyTransactions.First().Description,
                DateCreated = maxDateCreated,
                Amount = totalAmount,
                TransactionType = TransactionItemType.Declaration,
                TransactionDate = maxTransactionDate,
                SubTransactions = levyTransactions
            };


            return levyTransaction;
        }

    }
}