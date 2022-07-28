using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Commands.RejectTransferConnectionInvitation
{
    public class RejectTransferConnectionInvitationCommandHandler : AsyncRequestHandler<RejectTransferConnectionInvitationCommand>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserAccountRepository _userRepository;

        public RejectTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserAccountRepository userRepository)
        {
            _employerAccountRepository = employerAccountRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(RejectTransferConnectionInvitationCommand message)
        {
            var rejectorAccount = await _employerAccountRepository.Get(message.AccountId);
            var rejectorUser = await _userRepository.Get(message.UserRef);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.Get(message.TransferConnectionInvitationId.Value);

            transferConnectionInvitation.Reject(rejectorAccount, rejectorUser);
        }
    }
}