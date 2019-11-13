using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerFinance.Events.ProcessDeclaration
{
    public class ProcessDeclarationsEventHandler : IAsyncNotificationHandler<ProcessDeclarationsEvent>
    {
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly ILog _logger;
        private readonly IEventPublisher _eventPublisher;

        public ProcessDeclarationsEventHandler(
            IDasLevyRepository dasLevyRepository,
            ILog logger,
            IEventPublisher eventPublisher)
        {
            _dasLevyRepository = dasLevyRepository;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(ProcessDeclarationsEvent notification)
        {
            decimal levyTransactionTotalAmount
                =
            await _dasLevyRepository.ProcessDeclarations(notification.AccountId, notification.EmpRef);

            _logger.Info("Process Declarations Called");

            if (levyTransactionTotalAmount > decimal.Zero)
            {
                await
                _eventPublisher
                    .Publish(
                        new LevyAddedToAccount
                        {
                            AccountId = notification.AccountId,
                            Amount = levyTransactionTotalAmount
                        });
            }
        }
    }
}