﻿using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Queries.GetRejectedTransferConnectionInvitation
{
    public class GetRejectedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetRejectedTransferConnectionInvitationQuery, GetRejectedTransferConnectionInvitationResponse>
    {
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;

        public GetRejectedTransferConnectionInvitationQueryHandler(ITransferConnectionInvitationRepository transferConnectionInvitationRepository)
        {
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
        }

        public async Task<GetRejectedTransferConnectionInvitationResponse> Handle(GetRejectedTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetByReceiver(
                message.TransferConnectionInvitationId.Value,
                message.AccountId,
                TransferConnectionInvitationStatus.Rejected);

            return new GetRejectedTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = Mapper.Map<TransferConnectionInvitationDto>(transferConnectionInvitation)
            };
        }
    }
}