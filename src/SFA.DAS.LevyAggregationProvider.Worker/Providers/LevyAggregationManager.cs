﻿using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Commands.CreateLevyAggregation;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregationManager
    {
        [QueueName]
        public string refresh_employer_levy { get; set; }

        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public LevyAggregationManager(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILogger logger)
        {
            if (pollingMessageReceiver == null)
                throw new ArgumentNullException(nameof(pollingMessageReceiver));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Process()
        {

            var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyQueueMessage>();

            if (message?.Content != null && message.Content.AccountId != 0)
            {
                _logger.Info($"Processing LevyAggregation for Account: {message.Content.AccountId}");

                var response = await _mediator.SendAsync(new GetLevyDeclarationRequest
                {
                    AccountId = message.Content.AccountId
                });

                var aggregator = new LevyAggregator();

                var destinationData = aggregator.BuildAggregate(response.Data);

                if (destinationData != null)
                {
                    await _mediator.SendAsync(new CreateLevyAggregationCommand
                    {
                        Data = destinationData
                    });
                }
            }

            if (message != null)
            {
                await message.CompleteAsync();
            }   
            

        }
    }
}