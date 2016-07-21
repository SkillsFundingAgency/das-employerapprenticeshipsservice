using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserInvitations
{
    public class GetUserInvitationsQueryHandler : IAsyncRequestHandler<GetUserInvitationsRequest, GetUserInvitationsResponse>
    {
        private readonly IInvitationRepository _invitationRepository;

        public GetUserInvitationsQueryHandler(IInvitationRepository invitationRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            _invitationRepository = invitationRepository;
        }

        public async Task<GetUserInvitationsResponse> Handle(GetUserInvitationsRequest message)
        {
            var invitations = await _invitationRepository.Get(message.UserId);

            return new GetUserInvitationsResponse
            {
                Invitations = invitations
            };
        }
    }
}