using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnection
{
    public class SendTransferConnectionCommandHandler : AsyncRequestHandler<SendTransferConnectionCommand>
    {
        private readonly CurrentUser _currentUser;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionRepository _transferConnectionRepository;
        private readonly IMessagePublisher _messagePublisher;

        public SendTransferConnectionCommandHandler(
            CurrentUser currentUser,
            IMembershipRepository membershipRepository,
            ITransferConnectionRepository transferConnectionRepository,
            IMessagePublisher messagePublisher)
        {
            _currentUser = currentUser;
            _membershipRepository = membershipRepository;
            _transferConnectionRepository = transferConnectionRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(SendTransferConnectionCommand message)
        {
            var transferConnection = await _transferConnectionRepository.GetCreatedTransferConnection(message.TransferConnectionId.Value);
            var user = await _membershipRepository.GetCaller(transferConnection.SenderAccountId, _currentUser.ExternalUserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            transferConnection.Status = TransferConnectionStatus.Sent;

            await _transferConnectionRepository.Send(transferConnection);

            await _messagePublisher.PublishAsync(new TransferConnectionInvitationSentMessage(
                transferConnection.Id,
                transferConnection.SenderAccountId,
                transferConnection.ReceiverAccountId,
                user.FullName(),
                user.UserRef
            ));
        }
    }
}