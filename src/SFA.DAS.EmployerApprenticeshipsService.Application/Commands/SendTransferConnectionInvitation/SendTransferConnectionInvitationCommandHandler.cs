using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.SendTransferConnectionInvitation
{
    public class SendTransferConnectionInvitationCommandHandler : IAsyncRequestHandler<SendTransferConnectionInvitationCommand, long>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IPublicHashingService _publicHashingService;
        private readonly ITransferAllowanceService _transferAllowanceService;
        private readonly ITransferConnectionInvitationRepository _transferConnectionInvitationRepository;
        private readonly IUserRepository _userRepository;

        public SendTransferConnectionInvitationCommandHandler(
            IEmployerAccountRepository employerAccountRepository,
            IPublicHashingService publicHashingService,
            ITransferAllowanceService transferAllowanceService,
            ITransferConnectionInvitationRepository transferConnectionInvitationRepository,
            IUserRepository userRepository)
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
            var senderAccount = await _employerAccountRepository.GetAccountById(message.AccountId.Value);
            var receiverAccount = await _employerAccountRepository.GetAccountById(receiverAccountId);
            var senderUser = await _userRepository.GetUserById(message.UserId.Value);
            var senderAccountTransferAllowance = await _transferAllowanceService.GetTransferAllowance(message.AccountId.Value);
            var transferConnectionInvitation = senderAccount.SendTransferConnectionInvitation(receiverAccount, senderUser, senderAccountTransferAllowance);

            await _transferConnectionInvitationRepository.Add(transferConnectionInvitation);

            return transferConnectionInvitation.Id;
        }
    }
}