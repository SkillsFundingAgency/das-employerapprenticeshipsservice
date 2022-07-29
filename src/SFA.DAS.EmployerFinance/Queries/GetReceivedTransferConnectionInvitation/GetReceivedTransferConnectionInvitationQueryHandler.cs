using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Queries.GetReceivedTransferConnectionInvitation
{
    public class GetReceivedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetReceivedTransferConnectionInvitationQuery, GetReceivedTransferConnectionInvitationResponse>
    {
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IMapper _mapper;

        public GetReceivedTransferConnectionInvitationQueryHandler(ITransferConnectionInvitationRepository transferConnectionInvitationRepository, IMapper mapper)
        {
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _mapper = mapper;
        }

        public async Task<GetReceivedTransferConnectionInvitationResponse> Handle(GetReceivedTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetBySender(
                message.TransferConnectionInvitationId.Value,
                message.AccountId,
                TransferConnectionInvitationStatus.Pending);

            return new GetReceivedTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = _mapper.Map<TransferConnectionInvitationDto>(transferConnectionInvitation)
            };
        }
    }
}