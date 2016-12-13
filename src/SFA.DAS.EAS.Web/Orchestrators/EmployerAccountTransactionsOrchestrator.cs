using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountLevyDeclarationTransactions;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountPaymentTransactions;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAccountTransactionsOrchestrator
    {
        private readonly IMediator _mediator;

        public EmployerAccountTransactionsOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
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

            return new OrchestratorResponse<TransactionLineViewModel<LevyDeclarationTransactionLine>>
            {
                Status = HttpStatusCode.OK,
                Data = new TransactionLineViewModel<LevyDeclarationTransactionLine>
                {
                    Amount = data.Total,
                    SubTransactions = data.Transactions
                }
            };
        }

        public async Task<OrchestratorResponse<PaymentTransactionViewModel>> FindAccountPaymentTransactions(
            string hashedId, DateTime fromDate, DateTime toDate, string externalUserId)
        {
            try
            {
                var data = await _mediator.SendAsync(new FindEmployerAccountPaymentTransactionsQuery
                {
                    HashedAccountId = hashedId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    ExternalUserId = externalUserId
                });
                
                var courseGroups = data.Transactions.GroupBy(x => new { x.CourseName, x.CourseStartDate});

                var coursePaymentGroups = courseGroups.Select(x => new ApprenticeshipPaymentGroup
                {
                    ApprenticeCourseName = x.Key.CourseName,
                    StartDate = x.Key.CourseStartDate,
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

        public async Task<TransactionViewResult> GetAccountTransactions(string hashedId, string externalUserId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = hashedId,
                UserId = externalUserId
            });
            if (employerAccountResult.Account == null)
            {
                return new TransactionViewResult();
            }

            var data = await _mediator.SendAsync(new GetEmployerAccountTransactionsQuery {AccountId = employerAccountResult.Account.Id,ExternalUserId = externalUserId,HashedAccountId = hashedId});
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

            return new TransactionViewResult
            {
                Account = employerAccountResult.Account,
                Model = new TransactionViewModel
                {
                    CurrentBalance = currentBalance,
                    CurrentBalanceCalcultedOn = currentBalanceCalcultedOn,
                    Data = data.Data
                }
            };
        }
    }
}