using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetInvitation
{
    public class GetInvitationQueryHandler : IAsyncRequestHandler<GetInvitationRequest, GetInvitationResponse>
    {
        private readonly IInvitationRepository _invitationRepository;

        public GetInvitationQueryHandler(IInvitationRepository invitationRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            _invitationRepository = invitationRepository;
        }

        public async Task<GetInvitationResponse> Handle(GetInvitationRequest message)
        {
            var invitation = await _invitationRepository.GetView(message.Id);

            return new GetInvitationResponse
            {
                Invitation = invitation
            };
        }
    }
}