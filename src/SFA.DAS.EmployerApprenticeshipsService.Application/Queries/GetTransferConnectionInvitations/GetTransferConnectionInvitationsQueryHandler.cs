using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationsQuery, GetTransferConnectionInvitationsResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly IHashingService _hashingService;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;

        public GetTransferConnectionInvitationsQueryHandler(
            CurrentUser currentUser,
            IHashingService hashingService,
            IMembershipRepository membershipRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository)
        {
            _currentUser = currentUser;
            _hashingService = hashingService;
            _membershipRepository = membershipRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
        }

        public async Task<GetTransferConnectionInvitationsResponse> Handle(GetTransferConnectionInvitationsQuery message)
        {
            var membership = await _membershipRepository.GetCaller(message.HashedAccountId, _currentUser.ExternalUserId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }

            var senderAccountId = _hashingService.DecodeValue(message.HashedAccountId);
            var sentTransferConnectionInvitations = await _transferConnectionInvitationRepository.GetSentTransferConnectionInvitations(senderAccountId);

            return new GetTransferConnectionInvitationsResponse
            {
                TransferConnectionInvitations = sentTransferConnectionInvitations
            };
        }
    }
}