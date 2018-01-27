using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnections;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount
{
    public class GetTransferConnectionInvitationAccountQueryHandler : IAsyncRequestHandler<GetTransferConnectionInvitationAccountQuery, GetTransferConnectionInvitationAccountResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IHashingService _hashingService;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;

        public GetTransferConnectionInvitationAccountQueryHandler(
            CurrentUser currentUser,
            IEmployerAccountRepository employerAccountRepository,
            IHashingService hashingService,
            IMembershipRepository membershipRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository)
        {
            _currentUser = currentUser;
            _employerAccountRepository = employerAccountRepository;
            _hashingService = hashingService;
            _membershipRepository = membershipRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
        }

        public async Task<GetTransferConnectionInvitationAccountResponse> Handle(GetTransferConnectionInvitationAccountQuery message)
        {
            var membership = await _membershipRepository.GetCaller(message.SenderAccountHashedId, _currentUser.ExternalUserId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }

            var validationResult = new ValidationResult();
            var senderAccountId = _hashingService.DecodeValue(message.SenderAccountHashedId);
            var senderAccountTask = _employerAccountRepository.GetAccountById(senderAccountId);
            var receiverAccountTask = _employerAccountRepository.GetAccountByHashedId(message.ReceiverAccountHashedId);
            var senderAccount = await senderAccountTask;
            var receiverAccount = await receiverAccountTask;

            if (receiverAccount == null)
            {
                validationResult.AddError(nameof(message.ReceiverAccountHashedId), "Account ID cannot be found.");
            }
            else
            {
                var transferConnectionInvitations = await _transferConnectionInvitationRepository.GetTransferConnectionInvitations(senderAccount.Id, receiverAccount.Id);

                if (transferConnectionInvitations.Any(t => t.Status == TransferConnectionInvitationStatus.Sent))
                {
                    validationResult.AddError(nameof(message.ReceiverAccountHashedId), "You've already sent a connection request to this employer.");
                }
            }

            return new GetTransferConnectionInvitationAccountResponse
            {
                ReceiverAccount = receiverAccount,
                SenderAccount = senderAccount,
                ValidationResult = validationResult
            };
        }
    }
}