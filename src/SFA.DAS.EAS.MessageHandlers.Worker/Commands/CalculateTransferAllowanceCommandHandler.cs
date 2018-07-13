using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Interfaces;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.MessageHandlers.Worker.Commands
{
    [TopicSubscription("MA_CalculateTransferAllowanceSnapshotCommand")]
    public class CalculateTransferAllowanceCommandHandler : MessageProcessor<CalculateTransferAllowanceSnapshotCommand>
    {
        private readonly IDateService _datetimeService;
        private readonly ITransferAllowanceSnapshotRepository _transferAllowanceSnapshotRepository;
        private readonly IUnitOfWorkManagerFinance _unitOfWorkManager;
        private readonly IMediator _mediator;

        public CalculateTransferAllowanceCommandHandler(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log,
            IDateService datetimeService,
            ITransferAllowanceSnapshotRepository transferAllowanceSnapshotRepository,
            IUnitOfWorkManagerFinance unitOfWorkManager,
            IMediator mediator,
            IMessageContextProvider messageContextProvider) : base(subscriberFactory, log, messageContextProvider)
        {
            _datetimeService = datetimeService;
            _transferAllowanceSnapshotRepository = transferAllowanceSnapshotRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(CalculateTransferAllowanceSnapshotCommand messageContent)
        {
            try
            {
                var transferAllowanceResponse = await _mediator.SendAsync(new GetTransferAllowanceQuery {AccountId = messageContent.AccountId});

                await _transferAllowanceSnapshotRepository.UpsertAsync(messageContent.AccountId,
                    _datetimeService.CurrentFinancialYear.EndYear, transferAllowanceResponse.TransferAllowance);

                // Cause DB context to be saved and cached entities to be cleared
                _unitOfWorkManager.End();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to calculate transfer allowance snapshot for account {messageContent.AccountId}");
            }
        }
    }
}
