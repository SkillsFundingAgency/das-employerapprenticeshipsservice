using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.MarkerInterfaces;

namespace SFA.DAS.EmployerFinance.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommandHandler : IAsyncRequestHandler<SendTransferConnectionInvitationCommand, long>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly IUserAccountRepository _userRepository;
        private readonly IPublicHashingService _publicHashingService;
        private readonly EmployerFinanceConfiguration _configuration;

        public SendTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,            
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            ITransferRepository transferRepository,
            IUserAccountRepository userRepository,
            IPublicHashingService publicHashingService,
            EmployerFinanceConfiguration configuration)
        {
            _employerAccountRepository = employerAccountRepository;
            _transferConnectionInvitationRepository = transferConnectionInvitationRepository;
            _transferRepository = transferRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _publicHashingService = publicHashingService;
        }

        public async Task<long> Handle(SendTransferConnectionInvitationCommand message)
        {
            var receiverAccountId = _publicHashingService.DecodeValue(message.ReceiverAccountPublicHashedId);
            var senderAccount = await _employerAccountRepository.Get(message.AccountId);
            var receiverAccount = await _employerAccountRepository.Get(receiverAccountId);
            var senderUser = await _userRepository.Get(message.UserRef);
            var senderAccountTransferAllowance = await _transferRepository.GetTransferAllowance(message.AccountId, _configuration.TransferAllowancePercentage);
            var transferConnectionInvitation = senderAccount.SendTransferConnectionInvitation(receiverAccount, senderUser, senderAccountTransferAllowance.RemainingTransferAllowance ?? 0);

            await _transferConnectionInvitationRepository.Add(transferConnectionInvitation);

            return transferConnectionInvitation.Id;
        }
    }
}