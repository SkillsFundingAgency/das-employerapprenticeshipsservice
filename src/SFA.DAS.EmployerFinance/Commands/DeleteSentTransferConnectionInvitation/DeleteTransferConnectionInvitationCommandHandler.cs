using MediatR;
using SFA.DAS.EmployerFinance.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Commands.DeleteSentTransferConnectionInvitation
{
    public class DeleteTransferConnectionInvitationCommandHandler : AsyncRequestHandler<DeleteTransferConnectionInvitationCommand>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserRepository _userRepository;

        public DeleteTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserRepository userRepository)
        {
            _employerAccountRepository = employerAccountRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(DeleteTransferConnectionInvitationCommand message)
        {
            var deleterAccount = await _employerAccountRepository.GetAccountById(message.AccountId.Value);
            var deleterUser = await _userRepository.GetUserByRef(message.UserRef.Value);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetTransferConnectionInvitationById(message.TransferConnectionInvitationId.Value);

            transferConnectionInvitation.Delete(deleterAccount, deleterUser);
        }
    }
}