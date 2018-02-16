using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationsQuery, GetTransferConnectionInvitationsResponse>
    {
        private readonly EmployerAccountDbContext _db;

        public GetTransferConnectionInvitationsQueryHandler(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public async Task<GetTransferConnectionInvitationsResponse> Handle(GetTransferConnectionInvitationsQuery message)
        {
            var transferConnectionInvitations = await _db.TransferConnectionInvitations
                .Include(i => i.ReceiverAccount)
                .Include(i => i.SenderAccount)
                .Where(i => i.SenderAccount.Id == message.AccountId.Value || i.ReceiverAccount.Id == message.AccountId.Value)
                .OrderBy(i => i.CreatedDate)
                .ProjectTo<TransferConnectionInvitationDto>()
                .ToListAsync();

            return new GetTransferConnectionInvitationsResponse
            {
                AccountId = message.AccountId.Value,
                TransferConnectionInvitations = transferConnectionInvitations
            };
        }
    }
}