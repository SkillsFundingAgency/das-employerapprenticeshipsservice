using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnection
{
    public class SendTransferConnectionCommandHandler : AsyncRequestHandler<SendTransferConnectionCommand>
    {
        private readonly CurrentUser _currentUser;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionRepository _transferConnectionRepository;

        public SendTransferConnectionCommandHandler(
            CurrentUser currentUser,
            IMembershipRepository membershipRepository,
            ITransferConnectionRepository transferConnectionRepository)
        {
            _currentUser = currentUser;
            _membershipRepository = membershipRepository;
            _transferConnectionRepository = transferConnectionRepository;
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
        }
    }
}