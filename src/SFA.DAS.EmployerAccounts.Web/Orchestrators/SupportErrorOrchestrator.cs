using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class SupportErrorOrchestrator
{
    private readonly IMediator _mediator;

    public SupportErrorOrchestrator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public virtual async Task<OrchestratorResponse<AccountSummaryViewModel>> GetAccountSummary(string hashedAccountId, string externalUserId)
    {
        try
        {
            var accountResponse = await _mediator.Send(new GetEmployerAccountByHashedIdQuery
            {
                HashedAccountId = hashedAccountId,
                UserId = externalUserId
            });

            var viewModel = new AccountSummaryViewModel
            {
                Account = accountResponse.Account
            };

            return new OrchestratorResponse<AccountSummaryViewModel>
            {
                Status = HttpStatusCode.OK,
                Data = viewModel
            };
        }
        catch (InvalidRequestException ex)
        {
            return new OrchestratorResponse<AccountSummaryViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                Data = new AccountSummaryViewModel(),
                Exception = ex
            };
        }
        catch (UnauthorizedAccessException)
        {
            return new OrchestratorResponse<AccountSummaryViewModel>
            {
                Status = HttpStatusCode.Unauthorized
            };
        }
    }
}