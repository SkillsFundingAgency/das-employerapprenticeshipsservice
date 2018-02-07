using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation
{
    public class GetTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationQuery, GetTransferConnectionInvitationResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IHashingService _hashingService;

        public GetTransferConnectionInvitationQueryHandler(EmployerAccountDbContext db, IHashingService hashingService)
        {
            _db = db;
            _hashingService = hashingService;
        }

        public async Task<GetTransferConnectionInvitationResponse> Handle(GetTransferConnectionInvitationQuery message)
        {
            var accountId = _hashingService.DecodeValue(message.AccountHashedId);

            var transferConnectionInvitation = await _db.TransferConnectionInvitations
                .Include(i => i.ReceiverAccount)
                .Include(i => i.SenderAccount)
                .Include(i => i.SenderUser)
                .Where(i => 
                    i.Id == message.TransferConnectionInvitationId.Value && (
                    i.SenderAccount.Id == accountId ||
                    i.ReceiverAccount.Id == accountId))
                .ProjectTo<TransferConnectionInvitationDto>()
                .SingleOrDefaultAsync();

            if (transferConnectionInvitation == null)
            {
                return null;
            }

            return new GetTransferConnectionInvitationResponse
            {
                AccountId = accountId,
                TransferConnectionInvitation = transferConnectionInvitation
            };
        }
    }
}