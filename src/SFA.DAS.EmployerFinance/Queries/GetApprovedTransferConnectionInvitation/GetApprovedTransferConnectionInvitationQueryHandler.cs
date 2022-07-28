using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Queries.GetApprovedTransferConnectionInvitation
{
    public class GetApprovedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetApprovedTransferConnectionInvitationQuery, GetApprovedTransferConnectionInvitationResponse>
    {
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;

        public GetApprovedTransferConnectionInvitationQueryHandler(ITransferConnectionInvitationRepository transferConnectionInvitationRepository)
        {
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
        }

        public async Task<GetApprovedTransferConnectionInvitationResponse> Handle(GetApprovedTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetByReceiver(
                message.TransferConnectionInvitationId.Value,
                message.AccountId,
                TransferConnectionInvitationStatus.Pending);

            return new GetApprovedTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = Mapper.Map<TransferConnectionInvitationDto>(transferConnectionInvitation)
            };
        }
    }
}