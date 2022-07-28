using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationQuery, GetTransferConnectionInvitationResponse>
    {
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;

        public GetTransferConnectionInvitationQueryHandler(ITransferConnectionInvitationRepository transferConnectionInvitationRepository)
        {
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
        }

        public async Task<GetTransferConnectionInvitationResponse> Handle(GetTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetBySenderOrReceiver(
                message.TransferConnectionInvitationId.Value,
                message.AccountId);

            return new GetTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = Mapper.Map<TransferConnectionInvitationDto>(transferConnectionInvitation)
            };
        }
    }
}