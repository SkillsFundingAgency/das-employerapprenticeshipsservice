using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnection;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.CreateTransferConnectionInvitation
{
    public class CreateTransferConnectionInvitationCommandHandler : IAsyncRequestHandler<CreateTransferConnectionInvitationCommand, long>
    {
        private readonly CurrentUser _currentUser;
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;

        public CreateTransferConnectionInvitationCommandHandler(
            CurrentUser currentUser,
            IEmployerAccountRepository employerAccountRepository,
            IMembershipRepository membershipRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository)
        {
            _currentUser = currentUser;
            _employerAccountRepository = employerAccountRepository;
            _membershipRepository = membershipRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
        }

        public async Task<long> Handle(CreateTransferConnectionInvitationCommand message)
        {
            var user = await _membershipRepository.GetCaller(message.SenderHashedAccountId, _currentUser.ExternalUserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var receiverAccount = await _employerAccountRepository.GetAccountByHashedId(message.ReceiverHashedAccountId);

            if (receiverAccount == null)
            {
                throw new DomainException<CreateTransferConnectionInvitationCommand>(c => c.ReceiverHashedAccountId, "Account ID cannot be found.");
            }

            var senderAccount = await _employerAccountRepository.GetAccountByHashedId(message.SenderHashedAccountId);

            var transferConnectionInvitationId = await _transferConnectionInvitationRepository.Create(new TransferConnectionInvitation
            {
                SenderUserId = user.UserId,
                SenderAccountId = senderAccount.Id,
                ReceiverAccountId = receiverAccount.Id,
                Status = TransferConnectionInvitationStatus.Created,
                CreatedDate = DateTime.UtcNow
            });

            return transferConnectionInvitationId;
        }
    }
}