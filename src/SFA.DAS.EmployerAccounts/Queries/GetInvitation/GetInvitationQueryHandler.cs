using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetInvitation;

//TODO tests and validator
public class GetInvitationQueryHandler : IRequestHandler<GetInvitationRequest, GetInvitationResponse>
{
    private readonly IInvitationRepository _invitationRepository;

    public GetInvitationQueryHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
    }

    public async Task<GetInvitationResponse> Handle(GetInvitationRequest message, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetView(message.Id);

        return new GetInvitationResponse
        {
            Invitation = invitation
        };
    }
}