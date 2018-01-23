using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommandHandler : AsyncRequestHandler<SendTransferConnectionInvitationCommand>
    {
        private readonly CurrentUser _currentUser;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IMessagePublisher _messagePublisher;

        public SendTransferConnectionInvitationCommandHandler(
            CurrentUser currentUser,
            IMembershipRepository membershipRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IMessagePublisher messagePublisher)
        {
            _currentUser = currentUser;
            _membershipRepository = membershipRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(SendTransferConnectionInvitationCommand message)
        {
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetCreatedTransferConnectionInvitation(message.TransferConnectionInvitationId.Value);
            var user = await _membershipRepository.GetCaller(transferConnectionInvitation.SenderAccountId, _currentUser.ExternalUserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            transferConnectionInvitation.Status = TransferConnectionInvitationStatus.Sent;

            await _transferConnectionInvitationRepository.Send(transferConnectionInvitation);

            await _messagePublisher.PublishAsync(new TransferConnectionInvitationSentMessage(
                transferConnectionInvitation.Id,
                transferConnectionInvitation.SenderAccountId,
                transferConnectionInvitation.ReceiverAccountId,
                user.FullName(),
                user.UserRef
            ));
        }
    }
}