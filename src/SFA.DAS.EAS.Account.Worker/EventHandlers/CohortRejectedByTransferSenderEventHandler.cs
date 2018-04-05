using System.Threading.Tasks;
using SFA.DAS.Commitments.Events;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Messaging;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.EventHandlers
{
    [ServiceBusConnectionString("commitments")]
    [TopicSubscription("MA_CohortRejectedByTransferSender")]
    public class CohortRejectedByTransferSenderEventHandler : MessageProcessor<CohortRejectedByTransferSender>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly ITransferRequestRepository _transferRequestRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CohortRejectedByTransferSenderEventHandler(
            IEmployerAccountRepository employerAccountRepository,
            IMessageSubscriberFactory subscriberFactory,
            ILog log,
            ITransferRequestRepository transferRequestRepository,
            IUnitOfWorkManager unitOfWorkManager)
            : base(subscriberFactory, log)
        {
            _employerAccountRepository = employerAccountRepository;
            _transferRequestRepository = transferRequestRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected override async Task ProcessMessage(CohortRejectedByTransferSender message)
        {
            var senderAccount = await _employerAccountRepository.GetAccountById(message.SendingEmployerAccountId);
            var receiverAccount = await _employerAccountRepository.GetAccountById(message.ReceivingEmployerAccountId);
            var transferRequest = await  _transferRequestRepository.GetTransferRequestByCommitmentId(message.CommitmentId);

            transferRequest.Rejected(senderAccount, receiverAccount);

            _unitOfWorkManager.End();
        }
    }
}