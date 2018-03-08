using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetLatestOutstandingTransferInvitation
{
    public class GetLatestOutstandingTransferInvitationHandler : IAsyncRequestHandler<GetLatestOutstandingTransferInvitationQuery, GetLatestOutstandingTransferInvitationResponse>
    {
        private readonly IHashingService _hashingService;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IValidator<GetLatestOutstandingTransferInvitationQuery> _validator;

        public GetLatestOutstandingTransferInvitationHandler(
            IHashingService hashingService,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IValidator<GetLatestOutstandingTransferInvitationQuery> validator)
        {
            _hashingService = hashingService;
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