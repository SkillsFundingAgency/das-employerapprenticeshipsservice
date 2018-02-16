using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation
{
    public class GetReceivedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetReceivedTransferConnectionInvitationQuery, GetReceivedTransferConnectionInvitationResponse>
    {
        private readonly EmployerAccountDbContext _db;

        public GetReceivedTransferConnectionInvitationQueryHandler(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public async Task<GetReceivedTransferConnectionInvitationResponse> Handle(GetReceivedTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _db.TransferConnectionInvitations
                .Include(i => i.SenderAccount)
                .Where(i => 
                    i.Id == message.TransferConnectionInvitationId.Value &&
                    i.ReceiverAccount.Id == message.AccountId.Value &&
                    i.Status == TransferConnectionInvitationStatus.Pending
                )
                .ProjectTo<TransferConnectionInvitationDto>()
                .SingleOrDefaultAsync();

            if (transferConnectionInvitation == null)
            {
                return null;
            }

            return new GetReceivedTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = transferConnectionInvitation
            };
        }
    }
}