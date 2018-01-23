using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnectionInvitation
{
    public class GetCreatedTransferConnectionInvitationQueryHandler : IAsyncRequestHandler<GetCreatedTransferConnectionInvitationQuery, GetCreatedTransferConnectionInvitationResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;

        public GetCreatedTransferConnectionInvitationQueryHandler(
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

        public async Task<GetCreatedTransferConnectionInvitationResponse> Handle(GetCreatedTransferConnectionInvitationQuery message)
        {
            var transferConnection = await _transferConnectionInvitationRepository.GetCreatedTransferConnectionInvitation(message.TransferConnectionInvitationId.Value);

            if (transferConnection == null)
            {
                return null;
            }

            var user = await _membershipRepository.GetCaller(transferConnection.SenderAccountId, _currentUser.ExternalUserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }

            var receiverAccount = await _employerAccountRepository.GetAccountById(transferConnection.ReceiverAccountId);

            return new GetCreatedTransferConnectionInvitationResponse
            {
                ReceiverAccount = receiverAccount,
                TransferConnectionInvitation = transferConnection
            };
        }
    }
}