using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Queries.GetLatestPendingReceivedTransferConnectionInvitation
{
    public class GetLatestPendingReceivedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetLatestPendingReceivedTransferConnectionInvitationQuery, GetLatestPendingReceivedTransferConnectionInvitationResponse>
    {
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IMapper _mapper;

        public GetLatestPendingReceivedTransferConnectionInvitationQueryHandler(ITransferConnectionInvitationRepository transferConnectionInvitationRepository, IMapper mapper)
        {
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _mapper = mapper;
        }

        public async Task<GetLatestPendingReceivedTransferConnectionInvitationResponse> Handle(GetLatestPendingReceivedTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetLatestByReceiver(
                message.AccountId,
                TransferConnectionInvitationStatus.Pending);

            return new GetLatestPendingReceivedTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = _mapper.Map<TransferConnectionInvitationDto>(transferConnectionInvitation)
            };
        }
    }
}