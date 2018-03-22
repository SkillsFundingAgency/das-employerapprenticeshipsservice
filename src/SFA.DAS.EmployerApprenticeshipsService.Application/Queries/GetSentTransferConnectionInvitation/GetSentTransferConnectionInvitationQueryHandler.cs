using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation
{
    public class GetSentTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetSentTransferConnectionInvitationQuery, GetSentTransferConnectionInvitationResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetSentTransferConnectionInvitationQueryHandler(EmployerAccountDbContext db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetSentTransferConnectionInvitationResponse> Handle(GetSentTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _db.TransferConnectionInvitations
                .Where(i => 
                    i.Id == message.TransferConnectionInvitationId.Value &&
                    i.SenderAccount.Id == message.AccountId.Value &&
                    i.Status == TransferConnectionInvitationStatus.Pending
                )
                .ProjectTo<TransferConnectionInvitationDto>(_configurationProvider)
                .SingleOrDefaultAsync();

            if (transferConnectionInvitation == null)
            {
                return null;
            }

            return new GetSentTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = transferConnectionInvitation
            };
        }
    }
}