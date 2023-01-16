using System.Threading;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetInvitation;

//TODO tests and validator
public class GetInvitationQueryHandler : IRequestHandler<GetInvitationRequest, GetInvitationResponse>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IHashingService _hashingService;

    public GetInvitationQueryHandler(IInvitationRepository invitationRepository, IHashingService hashingService)
    {
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        _hashingService = hashingService;
    }

    public async Task<GetInvitationResponse> Handle(GetInvitationRequest message, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetView(_hashingService.DecodeValue(message.Id));

        return new GetInvitationResponse
        {
            Invitation = invitation
        };
    }
}