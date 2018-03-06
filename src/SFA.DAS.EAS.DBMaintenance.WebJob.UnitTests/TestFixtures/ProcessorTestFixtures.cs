using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor;
using SFA.DAS.EAS.DBMaintenance.WebJob.UnitTests.IdProcessor;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.DBMaintenance.WebJob.UnitTests.TestFixtures
{
    internal class ProcessorTestFixtures
    {
        public ProcessorTestFixtures()
        {
            CheckPointMock = new Mock<ICheckpoint>();
            IdProviderMock = new Mock<IIdProvider>();
            ProcessorMock = new Mock<IProcessor>();
            IdProcessorConfiguration = new IdProcessorConfiguration();
            EmployerApprenticeshipsServiceConfiguration = new EmployerApprenticeshipsServiceConfiguration();
            EmployerApprenticeshipsServiceConfiguration.IdProcessor = new IdProcessorConfiguration();

            LogMock = new Mock<ILog>();    
        }

        public ILog Log => LogMock.Object;
        public Mock<ILog> LogMock { get; set; }

        public ICheckpoint Checkpoint => CheckPointMock.Object;
        public Mock<ICheckpoint> CheckPointMock { get; set; }

        public IIdProvider IdProvider => IdProviderMock.Object;
        public Mock<IIdProvider> IdProviderMock { get; set; }


        public IProcessor Processor => ProcessorMock.Object;
        public Mock<IProcessor> ProcessorMock { get; set; }

        public EmployerApprenticeshipsServiceConfiguration EmployerApprenticeshipsServiceConfiguration { get; set; }

        public IdProcessorConfiguration IdProcessorConfiguration { get; set; }

        public ProcessorTestFixtures WithIds(IEnumerable<long> Ids)
        {
            var orderedIds = Ids.OrderBy(i => i);

            IdProviderMock
                .Setup(aip => aip.GetIdsAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<ProcessingContext>()))
                .Returns<long, int, ProcessingContext>((startAfter, count, processingContext) => Task.FromResult(orderedIds.Where(i => i > startAfter).Take(count)));

            return this;
        }

        public ProcessorTestFixtures WithIds(int startId, int IdCount)
        {
            return WithIds(Enumerable.Range(startId, IdCount).Select(Convert.ToInt64));
        }

        public ProcessorTestFixtures WithLoggedIds(int startId, int IdCount, List<BatchInfo> loggedBatches)
        {
            return WithLoggedIds(Enumerable.Range(startId, IdCount).Select(Convert.ToInt64), loggedBatches);
        }

        public ProcessorTestFixtures WithLoggedIds(IEnumerable<long> Ids, List<BatchInfo> loggedBatches)
        {
            var orderedIds = Ids.OrderBy(i => i);

            IdProviderMock
                .Setup(ap => ap.GetIdsAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<ProcessingContext>()))
                .Callback<long, int, ProcessingContext>((startAfterId, count, processingContext) => loggedBatches.Add(new BatchInfo(startAfterId, count)))
                .Returns<long, int, ProcessingContext>((startAfter, count, processingContext) => Task.FromResult(orderedIds.Where(i => i > startAfter).Take(count)));

            return this;
        }

        public ProcessorTestFixtures WithExactLoggedIds(IEnumerable<long> Ids, List<BatchInfo> loggedBatches)
        {
            IdProviderMock
                .Setup(ap => ap.GetIdsAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<ProcessingContext>()))
                .Callback<long, int, ProcessingContext>((startAfterId, count, processingContext) => loggedBatches.Add(new BatchInfo(startAfterId, count)))
                .Returns<long, int, ProcessingContext>((startAfter, count, processingContext) => Task.FromResult(Ids));

            return this;
        }

        public ProcessorTestFixtures WithAllProcessorCallsSuccessful()
        {
            ProcessorMock
                .Setup(ap => ap.DoAsync(It.IsAny<long>(), It.IsAny<ProcessingContext>()))
                .ReturnsAsync(true);

            return this;
        }

        public ProcessorTestFixtures WithProcessorCallsSuccessfulUntilSetPoint(long terminateWhenProcessingId)
        {
            ProcessorMock
                .Setup(ap => ap.DoAsync(It.IsAny<long>(), It.IsAny<ProcessingContext>()))
                .Returns<long, ProcessingContext>((id, processingContext) => Task.FromResult(id != terminateWhenProcessingId));

            return this;
        }

        public ProcessorTestFixtures WithAllProcessorCallsFaulting()
        {
            ProcessorMock
                .Setup(ap => ap.DoAsync(It.IsAny<long>(), It.IsAny<ProcessingContext>()))
                .Callback<long, ProcessingContext>((id, processingContext) => throw new Exception($"{id}"));

            return this;
        }

        public ProcessorTestFixtures WithAllProcessingFaultsIgnored()
        {
            ProcessorMock
                .Setup(ap => ap.InspectFailedAsync(It.IsAny<long>(), It.IsAny<Exception>(), It.IsAny<ProcessingContext>()))
                .ReturnsAsync(true);

            return this;
        }

        public ProcessorTestFixtures WithAnyProcessingFaultTerminatingProcess()
        {
            ProcessorMock
                .Setup(ap => ap.InspectFailedAsync(It.IsAny<long>(), It.IsAny<Exception>(), It.IsAny<ProcessingContext>()))
                .ReturnsAsync(false);

            return this;
        }

        public ProcessorTestFixtures WithLoggedCalls(List<long> idsProcessed)
        {
            ProcessorMock
                .Setup(ap => ap.DoAsync(It.IsAny<long>(), It.IsAny<ProcessingContext>()))
                .Callback<long, ProcessingContext>((id, processingContext) => idsProcessed.Add(id))
                .ReturnsAsync(true);

            return this;
        }

        public ProcessorTestFixtures WithLoggedCheckpointCalls(List<long> checkpointedIds)
        {
            CheckPointMock
                .Setup(ap => ap.SaveCheckpoint(IdProvider, It.IsAny<long>()))
                .Callback<IIdProvider, long>((idProvider, id) => checkpointedIds.Add(id));

            return this;
        }

        public ProcessorTestFixtures WithBatchSize(int batchSize)
        {
            IdProcessorConfiguration.BatchSize = batchSize;
            return this;
        }

        public ProcessorTestFixtures WithStartingAfterLastSuccess(long lastSuccessfulId)
        {
            CheckPointMock
                .Setup(aicp => aicp.GetLastCheckpointAsync(It.IsAny<IIdProvider>()))
                .ReturnsAsync(lastSuccessfulId);

            return this;
        }
    }
}