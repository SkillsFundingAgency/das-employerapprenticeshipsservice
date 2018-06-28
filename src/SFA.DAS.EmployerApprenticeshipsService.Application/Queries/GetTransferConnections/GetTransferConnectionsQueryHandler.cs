using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnections
{
    public class GetTransferConnectionsQueryHandler : IAsyncRequestHandler<GetTransferConnectionsQuery, GetTransferConnectionsResponse>
    {
        private readonly Lazy<EmployerAccountDbContext> _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetTransferConnectionsQueryHandler(Lazy<EmployerAccountDbContext> db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetTransferConnectionsResponse> Handle(GetTransferConnectionsQuery message)
        {
            var transferConnections = await _db.Value.TransferConnectionInvitations
                .Where(i => i.ReceiverAccount.Id == message.AccountId.Value && i.Status == TransferConnectionInvitationStatus.Approved)
                .OrderBy(i => i.SenderAccount.Name)
                .ProjectTo<TransferConnectionViewModel>(_configurationProvider)
                .ToListAsync();

            return new GetTransferConnectionsResponse
            {
                TransferConnections = transferConnections
            };
        }
    }
}