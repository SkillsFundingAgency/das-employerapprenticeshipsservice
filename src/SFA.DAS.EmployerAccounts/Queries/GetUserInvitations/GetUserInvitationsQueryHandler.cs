using System.Threading;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;

public class GetUserInvitationsQueryHandler : IRequestHandler<GetUserInvitationsRequest, GetUserInvitationsResponse>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IHashingService _hashingService;

    public GetUserInvitationsQueryHandler(IInvitationRepository invitationRepository, IHashingService hashingService)
    {
        _invitationRepository = invitationRepository;
        _hashingService = hashingService;
    }

    public async Task<GetUserInvitationsResponse> Handle(GetUserInvitationsRequest message, CancellationToken cancellationToken)
    {
        var invitations = await _invitationRepository.Get(message.UserId);

        foreach (var invitation in invitations)
        {
            invitation.HashedAccountId = _hashingService.HashValue(invitation.Id);
        }

        return new GetUserInvitationsResponse
        {
            Invitations = invitations
        };
    }
}