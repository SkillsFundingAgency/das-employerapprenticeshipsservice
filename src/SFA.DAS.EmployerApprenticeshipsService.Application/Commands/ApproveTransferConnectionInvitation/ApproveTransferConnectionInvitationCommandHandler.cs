using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Commands.ApproveTransferConnectionInvitation
{
    public class ApproveTransferConnectionInvitationCommandHandler : AsyncRequestHandler<ApproveTransferConnectionInvitationCommand>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IHashingService _hashingService;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserRepository _userRepository;

        public ApproveTransferConnectionInvitationCommandHandler(
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

        protected override async Task HandleCore(ApproveTransferConnectionInvitationCommand message)
        {
            var approverAccountId = _hashingService.DecodeValue(message.AccountHashedId);
            var approverAccount = await _employerAccountRepository.GetAccountById(approverAccountId);
            var approverUser = await _userRepository.GetUserByExternalId(message.UserExternalId.Value);
            var transferConnectionInvitation = await _transferConnectionInvitationRepository.GetTransferConnectionInvitationToApproveOrReject(message.TransferConnectionInvitationId.Value, approverAccountId);

            transferConnectionInvitation.Approve(approverAccount, approverUser);
        }
    }
}