using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.CreateTransferConnection
{
    public class CreateTransferConnectionCommandHandler : IAsyncRequestHandler<CreateTransferConnectionCommand, long>
    {
        private readonly CurrentUser _currentUser;
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionRepository _transferConnectionRepository;

        public CreateTransferConnectionCommandHandler(
            CurrentUser currentUser,
            IEmployerAccountRepository employerAccountRepository,
            IMembershipRepository membershipRepository,
            ITransferConnectionRepository transferConnectionRepository)
        {
            _currentUser = currentUser;
            _employerAccountRepository = employerAccountRepository;
            _membershipRepository = membershipRepository;
            _transferConnectionRepository = transferConnectionRepository;
        }

        public async Task<long> Handle(CreateTransferConnectionCommand message)
        {
            var user = await _membershipRepository.GetCaller(message.SenderHashedAccountId, _currentUser.ExternalUserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var receiverAccount = await _employerAccountRepository.GetAccountByHashedId(message.ReceiverHashedAccountId);

            if (receiverAccount == null)
            {
                throw new CommandException<CreateTransferConnectionCommand>(c => c.ReceiverHashedAccountId, "Account ID cannot be found.");
            }

            var senderAccount = await _employerAccountRepository.GetAccountByHashedId(message.SenderHashedAccountId);

            var transferConnectionId = await _transferConnectionRepository.Create(new TransferConnection
            {
                SenderUserId = user.UserId,
                SenderAccountId = senderAccount.Id,
                ReceiverAccountId = receiverAccount.Id,
                Status = TransferConnectionStatus.Created,
                CreatedDate = DateTime.UtcNow
            });

            return transferConnectionId;
        }
    }
}