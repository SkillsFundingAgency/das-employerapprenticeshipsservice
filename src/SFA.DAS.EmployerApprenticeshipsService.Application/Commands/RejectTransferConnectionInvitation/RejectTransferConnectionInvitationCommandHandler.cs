using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Commands.RejectTransferConnectionInvitation
{
    public class RejectTransferConnectionInvitationCommandHandler : AsyncRequestHandler<RejectTransferConnectionInvitationCommand>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IHashingService _hashingService;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserRepository _userRepository;

        public RejectTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            IHashingService hashingService,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserRepository userRepository)
        {
            _employerAccountRepository = employerAccountRepository;
            _hashingService = hashingService;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(RejectTransferConnectionInvitationCommand message)
        {
            var rejectorAccountId = _hashingService.DecodeValue(message.AccountHashedId);
            var rejectorAccount = await _employerAccountRepository.GetAccountById(rejectorAccountId);
            var rejectorUser = await _userRepository.GetUserByExternalId(message.UserExternalId.Value);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetTransferConnectionInvitationToApproveOrReject(message.TransferConnectionInvitationId.Value, rejectorAccountId);

            transferConnectionInvitation.Reject(rejectorAccount, rejectorUser);
        }
    }
}