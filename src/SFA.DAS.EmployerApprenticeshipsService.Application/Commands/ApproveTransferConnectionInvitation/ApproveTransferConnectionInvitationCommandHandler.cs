using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Commands.ApproveTransferConnectionInvitation
{
    public class ApproveTransferConnectionInvitationCommandHandler : AsyncRequestHandler<ApproveTransferConnectionInvitationCommand>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserRepository _userRepository;

        public ApproveTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserRepository userRepository)
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