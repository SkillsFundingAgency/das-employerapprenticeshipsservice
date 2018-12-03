﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationQuery, GetTransferConnectionInvitationResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetTransferConnectionInvitationQueryHandler(Lazy<EmployerAccountsDbContext> db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetTransferConnectionInvitationResponse> Handle(GetTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _db.Value.TransferConnectionInvitations
                .Where(i =>
                    i.Id == message.TransferConnectionInvitationId.Value && (
                    i.SenderAccount.Id == message.AccountId.Value && !i.DeletedBySender ||
                    i.ReceiverAccount.Id == message.AccountId.Value))
                .ProjectTo<TransferConnectionInvitationDto>(_configurationProvider)
                .SingleOrDefaultAsync();

            if (transferConnectionInvitation == null)
            {
                return null;
            }

            return new GetTransferConnectionInvitationResponse
            {
                AccountId = message.AccountId.Value,
                TransferConnectionInvitation = transferConnectionInvitation
            };
        }
    }
}