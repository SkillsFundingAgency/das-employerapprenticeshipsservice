using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionQuery
{
    public class GetSentTransferConnectionQueryHandler : IAsyncRequestHandler<GetSentTransferConnectionQuery, SentTransferConnectionViewModel>
    {
        private readonly CurrentUser _currentUser;
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ITransferConnectionRepository _transferConnectionRepository;

        public GetSentTransferConnectionQueryHandler(
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

        public async Task<SentTransferConnectionViewModel> Handle(GetSentTransferConnectionQuery message)
        {
            var transferConnection = await _transferConnectionRepository.GetSentTransferConnection(message.TransferConnectionId.Value);

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

            return new SentTransferConnectionViewModel
            {
                ReceiverAccount = receiverAccount,
                TransferConnection = transferConnection
            };
        }
    }
}