using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Api.Types;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQueryHandler : IAsyncRequestHandler<GetTransferConnectionsQuery, GetTransferConnectionsResponse>
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;
        private readonly IMapper _mapper;

        public GetTransferConnectionsQueryHandler(Lazy<EmployerFinanceDbContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<GetTransferConnectionsResponse> Handle(GetTransferConnectionsQuery message)
        {
            var transferConnectionInvitations = await _db.Value.TransferConnectionInvitations
                .Where(i => i.ReceiverAccount.Id == message.AccountId && i.Status == TransferConnectionInvitationStatus.Approved)
                .OrderBy(i => i.SenderAccount.Name)
                .ToListAsync();

            return new GetTransferConnectionsResponse
            {
                TransferConnections = _mapper.Map<List<TransferConnectionInvitation>, List<TransferConnection>>(transferConnectionInvitations)
            };
        }
    }
}