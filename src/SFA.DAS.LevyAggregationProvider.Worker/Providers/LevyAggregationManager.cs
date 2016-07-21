using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.LevyAggregationProvider.Worker.Commands;
using SFA.DAS.LevyAggregationProvider.Worker.Commands.CreateLevyAggregation;
using SFA.DAS.LevyAggregationProvider.Worker.Queries.GetLevyDeclaration;
using SFA.DAS.Messaging;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregationManager
    {
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;

        public LevyAggregationManager(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator)
        {
            if (pollingMessageReceiver == null)
                throw new ArgumentNullException(nameof(pollingMessageReceiver));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
        }

        public async Task Process()
        {
            //TODO review
            while (true)
            {
                var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>();

                if (message?.Content == null || message.Content.AccountId == 0)
                    return;

                var response = await _mediator.SendAsync(new GetLevyDeclarationRequest
                {
                    AccountId = message.Content.AccountId
                });

                var aggregator = new LevyAggregator();

                var destinationData = aggregator.BuildAggregate(response.Data);

                if (destinationData != null)
                    await _mediator.SendAsync(new CreateLevyAggregationCommand
                    {
                        Data = destinationData
                    });

                await message.CompleteAsync();
            }
        }
    }
}