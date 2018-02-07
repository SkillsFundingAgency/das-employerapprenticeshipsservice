using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation
{
    public class GetReceivedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetReceivedTransferConnectionInvitationQuery, GetReceivedTransferConnectionInvitationResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IHashingService _hashingService;

        public GetReceivedTransferConnectionInvitationQueryHandler(EmployerAccountDbContext db, IHashingService hashingService)
        {
            _db = db;
            _hashingService = hashingService;
        }

        public async Task<GetReceivedTransferConnectionInvitationResponse> Handle(GetReceivedTransferConnectionInvitationQuery message)
        {
            var accountId = _hashingService.DecodeValue(message.AccountHashedId);

            var transferConnectionInvitation = await _db.TransferConnectionInvitations
                .Include(i => i.SenderAccount)
                .Where(i => 
                    i.Id == message.TransferConnectionInvitationId.Value &&
                    i.ReceiverAccount.Id == accountId &&
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