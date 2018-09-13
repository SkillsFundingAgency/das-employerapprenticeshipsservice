﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Repositories;
using SFA.DAS.HashingService;

namespace SFA.DAS.Queries.GetUserInvitations
{
    public class GetUserInvitationsQueryHandler : IAsyncRequestHandler<GetUserInvitationsRequest, GetUserInvitationsResponse>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IHashingService _hashingService;

        public GetUserInvitationsQueryHandler(IInvitationRepository invitationRepository, IHashingService hashingService)
        {
            _invitationRepository = invitationRepository;
            _hashingService = hashingService;
        }

        public async Task<GetUserInvitationsResponse> Handle(GetUserInvitationsRequest message)
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
}