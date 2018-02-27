using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Data;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationsQuery, GetTransferConnectionInvitationsResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly ITransferRepository _transferRepository;
        private readonly ILog _logger;

        public GetTransferConnectionInvitationsQueryHandler(EmployerAccountDbContext db, ITransferRepository transferRepository, ILog logger)
        {
            _db = db;
            _transferRepository = transferRepository;
            _logger = logger;
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

            _logger.Debug($"Getting transfer allowance for account ID {message.AccountHashedId}");

            var transferAllowance = await _transferRepository.GetTransferAllowance(message.AccountId.Value);

            _logger.Debug($"Retrieved transfer allowance of for account ID {message.AccountHashedId}");

            return new GetTransferConnectionInvitationsResponse
            {
                AccountId = message.AccountId.Value,
                TransferAllowance = transferAllowance,
                TransferConnectionInvitations = transferConnectionInvitations
            };
        }
    }
}