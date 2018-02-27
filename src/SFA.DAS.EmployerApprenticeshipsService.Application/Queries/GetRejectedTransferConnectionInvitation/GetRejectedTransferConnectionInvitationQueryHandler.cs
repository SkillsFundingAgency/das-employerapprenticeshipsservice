using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Queries.GetRejectedTransferConnectionInvitation
{
    public class GetRejectedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetRejectedTransferConnectionInvitationQuery, GetRejectedTransferConnectionInvitationResponse>
    {
        private readonly EmployerAccountDbContext _db;

        public GetRejectedTransferConnectionInvitationQueryHandler(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public async Task<GetRejectedTransferConnectionInvitationResponse> Handle(GetRejectedTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _db.TransferConnectionInvitations
                .Include(i => i.ReceiverAccount)
                .Where(i => 
                    i.Id == message.TransferConnectionInvitationId.Value &&
                    i.ReceiverAccount.Id == message.AccountId.Value &&
                    i.Status == TransferConnectionInvitationStatus.Rejected
                )
                .ProjectTo<TransferConnectionInvitationDto>()
                .SingleOrDefaultAsync();

            if (transferConnectionInvitation == null)
            {
                return null;
            }

            return new GetRejectedTransferConnectionInvitationResponse
            {
                TransferConnectionInvitation = transferConnectionInvitation
            };
        }
    }
}