using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.MarkerInterfaces;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EmployerFinance.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommandHandler : IAsyncRequestHandler<SendTransferConnectionInvitationCommand, long>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IPublicHashingService _publicHashingService;
        private readonly ITransferAllowanceService _transferAllowanceService;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserAccountRepository _userRepository;

        public SendTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            IPublicHashingService publicHashingService,
            ITransferAllowanceService transferAllowanceService,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserAccountRepository userRepository)
        {
            _employerAccountRepository = employerAccountRepository;
            _publicHashingService = publicHashingService;
            _transferAllowanceService = transferAllowanceService;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _userRepository = userRepository;
        }

        public async Task<long> Handle(SendTransferConnectionInvitationCommand message)
        {
            var receiverAccountId = _publicHashingService.DecodeValue(message.ReceiverAccountPublicHashedId);
            var senderAccount = await _employerAccountRepository.Get(message.AccountId);
            var receiverAccount = await _employerAccountRepository.Get(receiverAccountId);
            var senderUser = await _userRepository.Get(message.UserRef);
            var senderAccountTransferAllowance = await _transferAllowanceService.GetTransferAllowance(message.AccountId);
            var transferConnectionInvitation = senderAccount.SendTransferConnectionInvitation(receiverAccount, senderUser, senderAccountTransferAllowance.RemainingTransferAllowance ?? 0);

            await _transferConnectionInvitationRepository.Add(transferConnectionInvitation);

            return transferConnectionInvitation.Id;
        }
    }
}