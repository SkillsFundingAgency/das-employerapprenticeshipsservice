using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.GetLatestPendingReceivedTransferConnectionInvitation
{
    public class GetLatestPendingReceivedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetLatestPendingReceivedTransferConnectionInvitationQuery, GetLatestPendingReceivedTransferConnectionInvitationResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetLatestPendingReceivedTransferConnectionInvitationQueryHandler(EmployerAccountDbContext db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetLatestPendingReceivedTransferConnectionInvitationResponse> Handle(GetLatestPendingReceivedTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _db.TransferConnectionInvitations
                .Where(i => i.ReceiverAccountId == message.AccountId && i.Status == TransferConnectionInvitationStatus.Pending)
                .OrderByDescending(i => i.CreatedDate)
                .ProjectTo<TransferConnectionInvitationDto>(_configurationProvider)
                .FirstOrDefaultAsync();

            return new GetLatestPendingReceivedTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = transferConnectionInvitation
            };
        }
    }
}