using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations
{
    public class GetTransferConnectionInvitationsQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationsQuery, GetTransferConnectionInvitationsResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ILog _logger;
        private readonly ITransferRepository _transferRepository;

        public GetTransferConnectionInvitationsQueryHandler(EmployerAccountDbContext db, IConfigurationProvider configurationProvider, ILog logger, ITransferRepository transferRepository)
        {
            _db = db;
            _configurationProvider = configurationProvider;
            _logger = logger;
            _transferRepository = transferRepository;
        }

        public async Task<GetTransferConnectionInvitationsResponse> Handle(GetTransferConnectionInvitationsQuery message)
        {
            var transferConnectionInvitations = await _db.TransferConnectionInvitations
                .Where(i => i.SenderAccount.Id == message.AccountId.Value && !i.DeletedBySender || i.ReceiverAccount.Id == message.AccountId.Value)
                .OrderBy(i => i.CreatedDate)
                .ProjectTo<TransferConnectionInvitationDto>(_configurationProvider)
                .ToListAsync();

            _logger.Debug($"Getting transfer allowance for account ID {message.AccountId}");

            var transferAllowance = await _transferRepository.GetTransferAllowance(message.AccountId.Value);

            _logger.Debug($"Retrieved transfer allowance of for account ID {message.AccountId}");

            return new GetTransferConnectionInvitationsResponse
            {
                AccountId = message.AccountId.Value,
                TransferAllowance = transferAllowance,
                TransferConnectionInvitations = transferConnectionInvitations
            };
        }
    }
}