﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationsQuery, GetTransferConnectionInvitationsResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetTransferConnectionInvitationsQueryHandler(Lazy<EmployerAccountsDbContext> db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetTransferConnectionInvitationsResponse> Handle(GetTransferConnectionInvitationsQuery message)
        {
            var transferConnectionInvitations = await _db.Value.TransferConnectionInvitations
                .Where(i => i.SenderAccount.Id == message.AccountId.Value && !i.DeletedBySender || i.ReceiverAccount.Id == message.AccountId.Value && !i.DeletedByReceiver)
                .OrderBy(i => i.ReceiverAccount.Id == message.AccountId.Value ? i.SenderAccount.Name : i.ReceiverAccount.Name)
                .ThenBy(i => i.CreatedDate)
                .ProjectTo<TransferConnectionInvitationDto>(_configurationProvider)
                .ToListAsync();

            return new GetTransferConnectionInvitationsResponse
            {
                TransferConnectionInvitations = transferConnectionInvitations
            };
        }
    }
}