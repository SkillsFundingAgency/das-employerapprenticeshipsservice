﻿using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class SupportErrorOrchestrator
    {
        private readonly IMediator _mediator;

        public SupportErrorOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }


        public virtual async Task<OrchestratorResponse<AccountDashboardViewModel>> GetAccountSummary(string hashedAccountId, string externalUserId)
        {
            try
            {
                var accountResponse = await _mediator.SendAsync(new GetEmployerAccountByHashedIdQuery
                {
                    HashedAccountId = hashedAccountId,
                    UserId = externalUserId
                });

                var viewModel = new AccountDashboardViewModel
                {
                    Account = accountResponse.Account
                };

                return new OrchestratorResponse<AccountDashboardViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = viewModel
                };
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse<AccountDashboardViewModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    Data = new AccountDashboardViewModel(),
                    Exception = ex
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new OrchestratorResponse<AccountDashboardViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }
    }
}