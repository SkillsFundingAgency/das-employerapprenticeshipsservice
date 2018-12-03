﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Events.ProcessPayment
{
    public class ProcessPaymentEventHandler : IAsyncNotificationHandler<ProcessPaymentEvent>

    {
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly ILog _logger;

        public ProcessPaymentEventHandler(IDasLevyRepository dasLevyRepository, ILog logger)
        {
            _dasLevyRepository = dasLevyRepository;
            _logger = logger;
        }

        public async Task Handle(ProcessPaymentEvent notification)
        {
            await _dasLevyRepository.ProcessPaymentData(notification.AccountId);

            _logger.Info("Process Payments Called");
        }
    }
}