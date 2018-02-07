using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationsQuery, GetTransferConnectionInvitationsResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IHashingService _hashingService;

        public GetTransferConnectionInvitationsQueryHandler(EmployerAccountDbContext db, IHashingService hashingService)
        {
            _db = db;
            _hashingService = hashingService;
        }

        public async Task<GetTransferConnectionInvitationsResponse> Handle(GetTransferConnectionInvitationsQuery message)
        {
            var accountId = _hashingService.DecodeValue(message.AccountHashedId);

            var transferConnectionInvitations = await _db.TransferConnectionInvitations
                .Include(i => i.ReceiverAccount)
                .Include(i => i.SenderAccount)
                .Where(i => i.SenderAccount.Id == accountId || i.ReceiverAccount.Id == accountId)
                .OrderBy(i => i.CreatedDate)
                .ProjectTo<TransferConnectionInvitationDto>()
                .ToListAsync();

            return new GetTransferConnectionInvitationsResponse
            {
                AccountId = accountId,
                TransferConnectionInvitations = transferConnectionInvitations
            };
        }
    }
}