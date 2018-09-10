using MediatR;
using SFA.DAS.EmployerFinance.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Commands.RejectTransferConnectionInvitation
{
    public class RejectTransferConnectionInvitationCommandHandler : AsyncRequestHandler<RejectTransferConnectionInvitationCommand>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserRepository _userRepository;

        public RejectTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserRepository userRepository)
        {
            _employerAccountRepository = employerAccountRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(RejectTransferConnectionInvitationCommand message)
        {
            var rejectorAccount = await _employerAccountRepository.GetAccountById(message.AccountId.Value);
            var rejectorUser = await _userRepository.GetUserByRef(message.UserRef.Value);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetTransferConnectionInvitationById(message.TransferConnectionInvitationId.Value);

            transferConnectionInvitation.Reject(rejectorAccount, rejectorUser);
        }
    }
}