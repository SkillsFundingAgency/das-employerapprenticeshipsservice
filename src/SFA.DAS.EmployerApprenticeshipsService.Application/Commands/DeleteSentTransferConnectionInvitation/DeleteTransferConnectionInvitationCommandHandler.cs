using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Commands.DeleteSentTransferConnectionInvitation
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
            var deleterUser = await _userRepository.GetUserById(message.UserId.Value);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetTransferConnectionInvitationById(message.TransferConnectionInvitationId.Value);

            var mode = DeleteTransferConnectionInvitationMode.NotSpecified;
            if (transferConnectionInvitation.ReceiverAccountId == message.AccountId)
            {
                mode = DeleteTransferConnectionInvitationMode.ReceiverIsDeleting;
            }
            else if (transferConnectionInvitation.SenderAccountId == message.AccountId)
            {
                mode = DeleteTransferConnectionInvitationMode.SenderIsDeleting;
            }

            transferConnectionInvitation.Delete(deleterAccount, deleterUser, mode);
        }
    }
}