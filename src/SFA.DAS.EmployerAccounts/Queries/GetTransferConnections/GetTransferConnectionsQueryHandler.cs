using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQueryHandler : IAsyncRequestHandler<GetTransferConnectionsQuery, GetTransferConnectionsResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetTransferConnectionsQueryHandler(Lazy<EmployerAccountsDbContext> db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetTransferConnectionsResponse> Handle(GetTransferConnectionsQuery message)
        {
            var transferConnections = await _db.Value.TransferConnectionInvitations
                .Where(i => i.ReceiverAccount.HashedId == message.HashedAccountId  && i.Status == TransferConnectionInvitationStatus.Approved)
                .OrderBy(i => i.SenderAccount.Name)
                .ProjectTo<TransferConnection>(_configurationProvider)
                .ToListAsync();

            return new GetTransferConnectionsResponse
            {
                TransferConnections = transferConnections
            };
        }
    }
}