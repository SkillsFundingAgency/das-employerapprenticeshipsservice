using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Queries.GetSentTransferConnectionInvitation
{
    public class GetSentTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetSentTransferConnectionInvitationQuery, GetSentTransferConnectionInvitationResponse>
    {
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;

        public GetSentTransferConnectionInvitationQueryHandler(ITransferConnectionInvitationRepository transferConnectionInvitationRepository)
        {
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
        }

        public async Task<GetSentTransferConnectionInvitationResponse> Handle(GetSentTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetBySender(
                message.TransferConnectionInvitationId.Value,
                message.AccountId,
                TransferConnectionInvitationStatus.Pending);

            return new GetSentTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = Mapper.Map<TransferConnectionInvitationDto>(transferConnectionInvitation)
            };
        }
    }
}