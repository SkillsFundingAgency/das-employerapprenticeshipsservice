using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetLatestOutstandingTransferInvitation
{
    public class GetLatestOutstandingTransferInvitationHandler : IAsyncRequestHandler<GetLatestOutstandingTransferInvitationQuery, GetLatestOutstandingTransferInvitationResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly IHashingService _hashingService;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IValidator<GetLatestOutstandingTransferInvitationQuery> _validator;

        public GetLatestOutstandingTransferInvitationHandler(
            CurrentUser currentUser,
            IHashingService hashingService,
            IMembershipRepository membershipRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IValidator<GetLatestOutstandingTransferInvitationQuery> validator)
        {
            _currentUser = currentUser;
            _hashingService = hashingService;
            _membershipRepository = membershipRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _validator = validator;
        }

        public async Task<GetLatestOutstandingTransferInvitationResponse> Handle(GetLatestOutstandingTransferInvitationQuery message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var membership = await _membershipRepository.GetCaller(message.ReceiverAccountHashedId, _currentUser.ExternalUserId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }

            var receiverAccountId = _hashingService.DecodeValue(message.ReceiverAccountHashedId);

            var transferConnectionInvitation =
                await _transferConnectionInvitationRepository.GetLatestOutstandingTransferConnectionInvitation(receiverAccountId);
                                                            
            return new GetLatestOutstandingTransferInvitationResponse
            {
                TransferConnectionInvitation = transferConnectionInvitation
            };
        }
    }
}