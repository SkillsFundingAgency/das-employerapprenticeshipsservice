using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor
{
    public class Broker : IBroker
    {
        private readonly IdProcessorConfiguration _idProcessorConfiguration;
        private readonly ICheckpoint _idCheckpoint;
        private readonly ILog _log;

        public Broker(
            IdProcessorConfiguration idProcessorConfiguration,
            ICheckpoint idCheckpoint,
            ILog log)
        {
            _idCheckpoint = idCheckpoint;
            _idProcessorConfiguration = idProcessorConfiguration;
            _log = log;
        }

        public async Task<ProcessingInfo> ProcessAsync(
            IIdProvider idProvider, 
            IProcessor idProcessor)
        {
            
            var processingInfo = await CreateProcessingInfoAsync(idProvider);
            var processingContext = new ProcessingContext();

            while (await ProcessNextBatch(processingInfo, idProvider, idProcessor, processingContext))
            {
                
            }

            if (processingInfo.State == ProcessingState.InProgress)
            {
                processingInfo.State = ProcessingState.Success;
            }
            processingInfo.EndTime = DateTime.UtcNow;
            return processingInfo;
        }

        private async Task<ProcessingInfo> CreateProcessingInfoAsync(IIdProvider idProvider)
        {
            var lastSuccessfulAccountProcessed = await _idCheckpoint.GetLastCheckpointAsync(idProvider);

            var result = new ProcessingInfo
            {
                BatchSize = _idProcessorConfiguration.BatchSize,
                StartTime = DateTime.UtcNow,
                LastProcessedId = lastSuccessfulAccountProcessed,
                State = ProcessingState.InProgress
            };

            return result;
        }

        private async Task<bool> ProcessNextBatch(
            ProcessingInfo processingInfo,
            IIdProvider idProvider, 
            IProcessor processor,
            ProcessingContext processingContext)
        {
            var accountIds = await idProvider.GetIdsAsync(processingInfo.LastProcessedId, processingInfo.BatchSize, processingContext);
                
            var processedBatchSize = 0;
            long? previousId = null; 

            // the link stuff is here in case the provider did not respect the parameters that were passed to it.
            foreach (var id in accountIds.Where(i => i > processingInfo.LastProcessedId).Take(processingInfo.BatchSize))
            {
                AssertIdsInCorrectOrder(id, ref previousId, processingInfo);

                if (!await ProcessNextAccountIdAsync(processingInfo, id, processor, processingContext))
                {
                    break;
                }

                processedBatchSize++;
                _idCheckpoint.SaveCheckpoint(idProvider, id);
                processingInfo.LastProcessedId = id;
            }

            if (processedBatchSize > 0)
            {
                processingInfo.BatchesProcessed++;
            }

            processingInfo.IdsProcessed =+ processedBatchSize;
            return processedBatchSize == processingInfo.BatchSize;
        }

        private void AssertIdsInCorrectOrder(long id, ref long? previousId, ProcessingInfo processingInfo)
        {
            if (id <= previousId)
            {
                processingInfo.State = ProcessingState.IdsProvidedOutOfOrder;
                throw new ProcessorException($"The ids provided by the current {nameof(IIdProvider)} were not provided in ascending order. Current-id:{id} Previous-id:{previousId.Value}", processingInfo);
            }

            previousId = id;
        }

        private async Task<bool> ProcessNextAccountIdAsync(ProcessingInfo processingInfo, long id, IProcessor processor, ProcessingContext processingContext)
        {
            bool shouldContinue;
            try
            {
                shouldContinue = await processor.DoAsync(id, processingContext);
                if(!shouldContinue)
                {
                    processingInfo.State = ProcessingState.TerminatedEarly;
                }
            }
            catch (Exception e)
            {
                processingInfo.UnhandledExceptions.Add(e.Message);
                _log.Error(e, $"Failed when processing entity with id {id}");
                shouldContinue = await processor.InspectFailedAsync(id, e, processingContext);
                if(!shouldContinue)
                {
                    processingInfo.State = ProcessingState.TerminatedByErrorHandler;
                }
            }

            return shouldContinue;
        }
    }
}