using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationQuery, GetTransferConnectionInvitationResponse>
    {
        private readonly EmployerAccountDbContext _db;

        public GetTransferConnectionInvitationQueryHandler(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public async Task<GetTransferConnectionInvitationResponse> Handle(GetTransferConnectionInvitationQuery message)
        {
            var transferConnectionInvitation = await _db.TransferConnectionInvitations
                .Include(i => i.ReceiverAccount)
                .Include(i => i.SenderAccount)
                .Include(i => i.SenderUser)
                .Where(i => 
                    i.Id == message.TransferConnectionInvitationId.Value && (
                    i.SenderAccount.Id == message.AccountId.Value ||
                    i.ReceiverAccount.Id == message.AccountId.Value))
                .ProjectTo<TransferConnectionInvitationDto>()
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