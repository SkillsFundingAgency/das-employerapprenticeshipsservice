using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Events.ProcessPayment
{
    public class ProcessPaymentEventHandler : IAsyncNotificationHandler<ProcessPaymentEvent>

    {
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly ILogger _logger;

        public ProcessPaymentEventHandler(IDasLevyRepository dasLevyRepository, ILogger logger)
        {
            _dasLevyRepository = dasLevyRepository;
            _logger = logger;
        }

        public async Task Handle(ProcessPaymentEvent notification)
        {
            await _dasLevyRepository.ProcessPaymentData();

            _logger.Info("Process Payments Called");
        }
    }
}