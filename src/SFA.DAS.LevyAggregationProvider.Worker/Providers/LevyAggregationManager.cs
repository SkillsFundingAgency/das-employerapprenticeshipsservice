using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.Messaging;

namespace SFA.DAS.LevyAggregationProvider.Worker.Providers
{
    public class LevyAggregationManager
    {
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly ILevyDeclarationReader _levyDeclarationReader;
        private readonly ILevyAggregationWriter _levyAggregationWriter;

        public LevyAggregationManager(IPollingMessageReceiver pollingMessageReceiver, ILevyDeclarationReader levyDeclarationReader, ILevyAggregationWriter levyAggregationWriter)
        {
            if (pollingMessageReceiver == null)
                throw new ArgumentNullException(nameof(pollingMessageReceiver));
            if (levyDeclarationReader == null)
                throw new ArgumentNullException(nameof(levyDeclarationReader));
            if (levyAggregationWriter == null)
                throw new ArgumentNullException(nameof(levyAggregationWriter));
            _pollingMessageReceiver = pollingMessageReceiver;
            _levyDeclarationReader = levyDeclarationReader;
            _levyAggregationWriter = levyAggregationWriter;
        }

        public async Task Process()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<EmployerRefreshLevyAggregationQueueMessage>();

            if (message.Content.AccountId == 0)
                return;

            var sourceData = await _levyDeclarationReader.GetData(message.Content.AccountId);

            if (sourceData.AccountId == 0)
                return;

            var aggregator = new LevyAggregator();

            var destinationData = aggregator.BuildAggregate(sourceData);

            if (destinationData != null)
                await _levyAggregationWriter.UpdateAsync(destinationData);
        }
    }
}