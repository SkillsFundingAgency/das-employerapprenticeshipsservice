using System.Data.Entity;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<SendTransferConnectionInvitationQuery, SendTransferConnectionInvitationResponse>
    {
        private readonly EmployerAccountDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IPublicHashingService _publicHashingService;

        public SendTransferConnectionInvitationQueryHandler(
            EmployerAccountDbContext db,
            IConfigurationProvider configurationProvider,
            IPublicHashingService publicHashingService)
        {
            _db = db;
            _configurationProvider = configurationProvider;
            _publicHashingService = publicHashingService;
        }

        public async Task<SendTransferConnectionInvitationResponse> Handle(SendTransferConnectionInvitationQuery message)
        {
            var receiverAccountId = _publicHashingService.DecodeValue(message.ReceiverAccountPublicHashedId);
            var receiverAccount = await _db.Accounts.ProjectTo<AccountDto>(_configurationProvider).SingleOrDefaultAsync(a => a.Id == receiverAccountId);

            if (receiverAccount == null || receiverAccount.Id == message.AccountId)
            {
                throw new ValidationException<SendTransferConnectionInvitationQuery>(q => q.ReceiverAccountPublicHashedId, "You must enter a valid account ID");
            }

            var isReceiverASender = await _db.TransferConnectionInvitations.AnyAsync(i =>
                i.SenderAccountId == receiverAccount.Id && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            if (isReceiverASender)
            {
                throw new ValidationException<SendTransferConnectionInvitationQuery>(q => q.ReceiverAccountPublicHashedId, "You can't connect with this employer because they already have pending or accepted connection requests");
            }

            var anyTransferConnectionInvitations = await _db.TransferConnectionInvitations.AnyAsync(i => (
                i.SenderAccount.Id == message.AccountId.Value && i.ReceiverAccount.Id == receiverAccount.Id ||
                i.SenderAccount.Id == receiverAccount.Id && i.ReceiverAccount.Id == message.AccountId.Value) && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            if (anyTransferConnectionInvitations)
            {
                throw new ValidationException<SendTransferConnectionInvitationQuery>(q => q.ReceiverAccountPublicHashedId, "You can't connect with this employer because they already have a pending or accepted connection request");
            }

            return new SendTransferConnectionInvitationResponse
            {
                ReceiverAccount = receiverAccount
            };
        }
    }
}