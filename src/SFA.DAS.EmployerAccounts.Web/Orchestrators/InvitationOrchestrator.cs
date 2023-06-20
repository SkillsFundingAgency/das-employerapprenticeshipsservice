using SFA.DAS.EmployerAccounts.Commands.AcceptInvitation;
using SFA.DAS.EmployerAccounts.Commands.CreateInvitation;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetInvitation;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class InvitationOrchestrator
{
    private readonly IMediator _mediator;
    private readonly ILogger<InvitationOrchestrator> _logger;
    private readonly IEncodingService _encodingService;

    protected InvitationOrchestrator() { }

    public InvitationOrchestrator(IMediator mediator, ILogger<InvitationOrchestrator> logger, IEncodingService encodingService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger;
        _encodingService = encodingService;
    }

    public virtual async Task<OrchestratorResponse<InvitationView>> GetInvitation(string hashedId)
    {
        var invitationId = _encodingService.Decode(hashedId, EncodingType.AccountId);
        var response = await _mediator.Send(new GetInvitationRequest
        {
            Id = invitationId
        });

        var result = new OrchestratorResponse<InvitationView>
        {
            Data = response.Invitation
        };

        return result;
    }

    public virtual async Task AcceptInvitation(long invitationId, string externalUserId)
    {
        await _mediator.Send(new AcceptInvitationCommand
        {
            Id = invitationId,
            ExternalUserId = externalUserId
        });
    }

    public async Task CreateInvitation(InviteTeamMemberViewModel model, string externalUserId)
    {
        try
        {
            await _mediator.Send(new CreateInvitationCommand
            {
                HashedAccountId = model.HashedAccountId,
                ExternalUserId = externalUserId,
                NameOfPersonBeingInvited = model.Name,
                EmailOfPersonBeingInvited = model.Email,
                RoleOfPersonBeingInvited = model.Role
            });
        }
        catch (InvalidRequestException ex)
        {
            _logger.LogError(ex, "Exception caught in CreateInvitation method.");
        }

    }

    public async Task<OrchestratorResponse<UserInvitationsViewModel>> GetAllInvitationsForUser(string externalUserId)
    {
        try
        {
            var response = await _mediator.Send(new GetUserInvitationsRequest
            {
                UserId = externalUserId
            });

            var getUserAccountsQueryResponse = await _mediator.Send(new GetUserAccountsQuery
            {
                UserRef = externalUserId
            });

            var result = new OrchestratorResponse<UserInvitationsViewModel>
            {
                Data = new UserInvitationsViewModel
                {
                    Invitations = response.Invitations,
                    ShowBreadCrumbs = getUserAccountsQueryResponse.Accounts.AccountList.Count != 0
                }
            };

            return result;
        }
        catch (InvalidRequestException ex)
        {
            _logger.LogError(ex, "Exception caught in GetAllInvitationsForUser method.");
        }

        return new OrchestratorResponse<UserInvitationsViewModel>
        {
            Data = null
        };
    }
}