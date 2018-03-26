﻿using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Dtos;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationsQuery, GetTransferConnectionInvitationsResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetTransferConnectionInvitationsQueryHandler(EmployerAccountDbContext db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetTransferConnectionInvitationsResponse> Handle(GetTransferConnectionInvitationsQuery message)
        {
            var transferConnectionInvitations = await _db.TransferConnectionInvitations
                .Where(i => i.SenderAccount.Id == message.AccountId.Value && !i.DeletedBySender || i.ReceiverAccount.Id == message.AccountId.Value)
                .OrderBy(i => i.CreatedDate)
                .ProjectTo<TransferConnectionInvitationDto>(_configurationProvider)
                .ToListAsync();

            return new GetTransferConnectionInvitationsResponse
            {
                AccountId = message.AccountId.Value,
                TransferConnectionInvitations = transferConnectionInvitations
            };
        }
    }
}