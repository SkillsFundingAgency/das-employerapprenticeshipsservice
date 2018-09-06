using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Models.TransferConnections;
using SFA.DAS.Hashing;
using SFA.DAS.Validation;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<SendTransferConnectionInvitationQuery, SendTransferConnectionInvitationResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IPublicHashingService _publicHashingService;

        public SendTransferConnectionInvitationQueryHandler(
            Lazy<EmployerAccountsDbContext> db,
            IConfigurationProvider configurationProvider,
            IPublicHashingService publicHashingService)
        {
            _db = db;
            _configurationProvider = configurationProvider;
            _publicHashingService = publicHashingService;
        }

        public async Task<SendTransferConnectionInvitationResponse> Handle(SendTransferConnectionInvitationQuery message)
        {
            var receiverAccount = _publicHashingService.TryDecodeValue(message.ReceiverAccountPublicHashedId, out var receiverAccountId)
                ? await _db.Value.Accounts.ProjectTo<AccountDto>(_configurationProvider).SingleOrDefaultAsync(a => a.Id == receiverAccountId)
                : null;

            if (receiverAccount == null || receiverAccount.Id == message.AccountId)
            {
                throw new ValidationException<SendTransferConnectionInvitationQuery>(q => q.ReceiverAccountPublicHashedId, "You must enter a valid account ID");
            }

            var isReceiverASender = await _db.Value.TransferConnectionInvitations.AnyAsync(i =>
                i.SenderAccountId == receiverAccount.Id && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            if (isReceiverASender)
            {
                throw new ValidationException<SendTransferConnectionInvitationQuery>(q => q.ReceiverAccountPublicHashedId, "You can't connect with this employer because they already have pending or accepted connection requests");
            }

            var anyTransferConnectionInvitations = await _db.Value.TransferConnectionInvitations.AnyAsync(i => (
                i.SenderAccount.Id == message.AccountId.Value && i.ReceiverAccount.Id == receiverAccount.Id ||
                i.SenderAccount.Id == receiverAccount.Id && i.ReceiverAccount.Id == message.AccountId.Value) && (
                i.Status == TransferConnectionInvitationStatus.Pending ||
                i.Status == TransferConnectionInvitationStatus.Approved));

            if (anyTransferConnectionInvitations)
            {
                throw new ValidationException<SendTransferConnectionInvitationQuery>(q => q.ReceiverAccountPublicHashedId, "You can't connect with this employer because they already have a pending or accepted connection request");
            }

            var senderAccount = await _db.Value.Accounts.ProjectTo<AccountDto>(_configurationProvider).SingleOrDefaultAsync(a => a.Id == message.AccountId);

            return new SendTransferConnectionInvitationResponse
            {
                ReceiverAccount = receiverAccount,
                SenderAccount = senderAccount,
            };
        }
    }
}