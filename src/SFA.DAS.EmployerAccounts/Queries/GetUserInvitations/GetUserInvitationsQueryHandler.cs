using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;

public class GetUserInvitationsQueryHandler : IRequestHandler<GetUserInvitationsRequest, GetUserInvitationsResponse>
{
    private readonly IInvitationRepository _invitationRepository;

    public GetUserInvitationsQueryHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<GetUserInvitationsResponse> Handle(GetUserInvitationsRequest message, CancellationToken cancellationToken)
    {
        var invitations = await _invitationRepository.Get(message.UserId);

        return new GetUserInvitationsResponse
        {
            Invitations = invitations
        };
    }
}