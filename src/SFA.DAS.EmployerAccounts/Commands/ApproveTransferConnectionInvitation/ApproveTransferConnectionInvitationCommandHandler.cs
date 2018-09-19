using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Commands.ApproveTransferConnectionInvitation
{
    public class ApproveTransferConnectionInvitationCommandHandler : AsyncRequestHandler<ApproveTransferConnectionInvitationCommand>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserAccountRepository _userRepository;

        public ApproveTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserAccountRepository userRepository)
        {
            _employerAccountRepository = employerAccountRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(ApproveTransferConnectionInvitationCommand message)
        {
            var approverAccount = await _employerAccountRepository.GetAccountById(message.AccountId.Value);
            var approverUser = await _userRepository.GetUserByRef(message.UserRef.Value);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetTransferConnectionInvitationById(message.TransferConnectionInvitationId.Value);

            transferConnectionInvitation.Approve(approverAccount, approverUser);
        }
    }
}