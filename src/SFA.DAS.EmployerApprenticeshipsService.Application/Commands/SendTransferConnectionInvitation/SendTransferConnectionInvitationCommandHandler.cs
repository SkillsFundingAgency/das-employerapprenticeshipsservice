using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommandHandler : IAsyncRequestHandler<SendTransferConnectionInvitationCommand, long>
    {
        private readonly CurrentUser _currentUser;
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IHashingService _hashingService;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IPublicHashingService _publicHashingService;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IMessagePublisher _messagePublisher;

        public SendTransferConnectionInvitationCommandHandler(
            CurrentUser currentUser,
            IEmployerAccountRepository employerAccountRepository,
            IHashingService hashingService,
            IMembershipRepository membershipRepository,
            IPublicHashingService publicHashingService,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IMessagePublisher messagePublisher)
        {
            _currentUser = currentUser;
            _employerAccountRepository = employerAccountRepository;
            _hashingService = hashingService;
            _membershipRepository = membershipRepository;
            _publicHashingService = publicHashingService;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<long> Handle(SendTransferConnectionInvitationCommand message)
        {
            var membership = await _membershipRepository.GetCaller(message.SenderAccountHashedId, _currentUser.ExternalUserId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }

            var receiverAccountId = _publicHashingService.DecodeValue(message.ReceiverAccountPublicHashedId);
            var senderAccountId = _hashingService.DecodeValue(message.SenderAccountHashedId);
            var senderAccountTask = _employerAccountRepository.GetAccountById(senderAccountId);
            var receiverAccountTask = _employerAccountRepository.GetAccountById(receiverAccountId);
            var senderAccount = await senderAccountTask;
            var receiverAccount = await receiverAccountTask;

            var transferConnectionInvitation = new TransferConnectionInvitation
            {
                SenderUserId = membership.UserId,
                SenderAccountId = senderAccount.Id,
                ReceiverAccountId = receiverAccount.Id,
                Status = TransferConnectionInvitationStatus.Sent,
                CreatedDate = DateTime.UtcNow
            };

            var transferConnectionInvitationId = await _transferConnectionInvitationRepository.Add(transferConnectionInvitation);

            await _messagePublisher.PublishAsync(new TransferConnectionInvitationSentMessage(
                transferConnectionInvitationId,
                transferConnectionInvitation.SenderAccountId,
                senderAccount.Name,
                transferConnectionInvitation.ReceiverAccountId,
                receiverAccount.Name,
                membership.FullName(),
                membership.UserRef
            ));

            return transferConnectionInvitationId;
        }
    }
}