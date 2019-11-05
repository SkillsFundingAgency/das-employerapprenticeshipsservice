﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitations
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
                .Where(i => i.SenderAccount.Id == message.AccountId && !i.DeletedBySender || i.ReceiverAccount.Id == message.AccountId && !i.DeletedByReceiver)
                .OrderBy(i => i.ReceiverAccount.Id == message.AccountId ? i.SenderAccount.Name : i.ReceiverAccount.Name)
                .ThenBy(i => i.CreatedDate)
                .ProjectTo<TransferConnectionInvitationDto>(_configurationProvider)
                .ToListAsync();

            return new GetTransferConnectionInvitationsResponse
            {
                TransferConnectionInvitations = transferConnectionInvitations,
                AccountId = message.AccountId
            };
        }
    }
}