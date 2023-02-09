using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public abstract class UserVerificationOrchestratorBase
{
    public IMediator Mediator { get; set; }

    protected UserVerificationOrchestratorBase() { }

    protected UserVerificationOrchestratorBase(IMediator mediator)
    {
        Mediator = mediator;
    }

    public virtual async Task<GetUserAccountRoleResponse> GetUserAccountRole(long accountId, string externalUserId)
    {
        return await Mediator.Send(new GetUserAccountRoleQuery
        {
            AccountId = accountId,
            ExternalUserId = externalUserId
        });
    }
}