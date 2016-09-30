using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetInvitation
{
    //TODO tests and validator
    public class GetInvitationQueryHandler : IAsyncRequestHandler<GetInvitationRequest, GetInvitationResponse>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IHashingService _hashingService;

        public GetInvitationQueryHandler(IInvitationRepository invitationRepository, IHashingService hashingService)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            _invitationRepository = invitationRepository;
            _hashingService = hashingService;
        }

        public async Task<GetInvitationResponse> Handle(GetInvitationRequest message)
        {
            var invitation = await _invitationRepository.GetView(_hashingService.DecodeValue(message.Id));

            return new GetInvitationResponse
            {
                Invitation = invitation
            };
        }
    }
}