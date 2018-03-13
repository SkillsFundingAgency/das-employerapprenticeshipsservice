using System.Threading.Tasks;
using SFA.DAS.Commitments.Events;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Messaging;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.EventHandlers
{
    [ServiceBusConnectionString("commitments")]
    [TopicSubscription("MA_CohortApprovalByTransferSenderRequested")]
    public class CohortApprovalByTransferSenderRequestedEventHandler : MessageProcessor<CohortApprovalByTransferSenderRequested>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IHashingService _hashingService;
        private readonly ITransferRequestRepository _transferRequestRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CohortApprovalByTransferSenderRequestedEventHandler(
            IEmployerAccountRepository employerAccountRepository,
            IHashingService hashingService,
            IMessageSubscriberFactory subscriberFactory,
            ILog log,
            ITransferRequestRepository transferRequestRepository,
            IUnitOfWorkManager unitOfWorkManager)
            : base(subscriberFactory, log)
        {
            _employerAccountRepository = employerAccountRepository;
            _hashingService = hashingService;
            _transferRequestRepository = transferRequestRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected override async Task ProcessMessage(CohortApprovalByTransferSenderRequested message)
        {
            var commitmentHashedId = _hashingService.HashValue(message.CommitmentId);
            var senderAccount = await _employerAccountRepository.GetAccountById(message.SendingEmployerAccountId);
            var receiverAccount = await _employerAccountRepository.GetAccountById(message.ReceivingEmployerAccountId);
            var transferRequest = receiverAccount.SentTransferRequest(senderAccount, message.CommitmentId, commitmentHashedId, message.TransferCost);

            await _transferRequestRepository.Add(transferRequest);

            _unitOfWorkManager.End();
        }
    }
}