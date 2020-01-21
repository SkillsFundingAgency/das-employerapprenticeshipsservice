using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class SupportErrorOrchestrator
    {
        private readonly IMediator _mediator;

        public SupportErrorOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }


        public virtual async Task<OrchestratorResponse<AccountDashboardViewModel>> GetAccount(string hashedAccountId, string externalUserId)
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

                //note: ApprenticeshipEmployerType is already returned by GetEmployerAccountHashedQuery, but we need to transition to calling the api instead.
                // we could blat over the existing flag, but it's much nicer to store the enum (as above) rather than a byte!
                //viewModel.Account.ApprenticeshipEmployerType = (byte) ((ApprenticeshipEmployerType) Enum.Parse(typeof(ApprenticeshipEmployerType), apiGetAccountTask.Result.ApprenticeshipEmployerType, true));

                return new OrchestratorResponse<AccountDashboardViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = viewModel
                };
            }
        }
    }
}