using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Commands.DeleteSentTransferConnectionInvitation
{
    public class DeleteTransferConnectionInvitationCommandHandler : AsyncRequestHandler<DeleteTransferConnectionInvitationCommand>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserAccountRepository _userRepository;

        public DeleteTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserAccountRepository userRepository)
        {
            _employerAccountRepository = employerAccountRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(DeleteTransferConnectionInvitationCommand message)
        {
            var deleterAccount = await _employerAccountRepository.Get(message.AccountId);
            var deleterUser = await _userRepository.Get(message.UserRef);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.Get(message.TransferConnectionInvitationId.Value);

            transferConnectionInvitation.Delete(deleterAccount, deleterUser);
        }
    }
}