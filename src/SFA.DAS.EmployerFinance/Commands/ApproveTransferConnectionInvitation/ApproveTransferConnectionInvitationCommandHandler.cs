using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Commands.ApproveTransferConnectionInvitation
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
            var approverAccount = await _employerAccountRepository.Get(message.AccountId);
            var approverUser = await _userRepository.Get(message.UserRef);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.Get(message.TransferConnectionInvitationId.Value);

            transferConnectionInvitation.Approve(approverAccount, approverUser);
        }
    }
}