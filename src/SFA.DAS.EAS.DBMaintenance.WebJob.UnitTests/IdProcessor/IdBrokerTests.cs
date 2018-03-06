using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor;
using SFA.DAS.EAS.DBMaintenance.WebJob.UnitTests.TestFixtures;

namespace SFA.DAS.EAS.DBMaintenance.WebJob.UnitTests.IdProcessor
{
    [TestFixture]
    public class IdBrokerTests
    {
        [Test]
        public void Constructor_Valid_ShouldSucceed()
        {
            var fixtures = new ProcessorTestFixtures();

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            Assert.Pass("Shouldn't throw exception");
        }


        [TestCase]
        [TestCase(1)]
        [TestCase(1,2,3,4,5)]
        public async Task ProcessAsync_WithSpecifiedNumberOfInputs_ShouldCallDoAsyncSameNumberOfTimes(params long[] availableIds)
        {
            // Arrange
            List<long> processedIds = new List<long>();

            var fixtures = new ProcessorTestFixtures()
                .WithIds(availableIds)
                .WithLoggedCalls(processedIds);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            var expectedNumberOfCalls = availableIds.Length;
            Assert.AreEqual(expectedNumberOfCalls, processedIds.Count);
        }

        [TestCase()]
        [TestCase(1)]
        [TestCase(1,2,3,4,5)]
        public async Task ProcessAsync_WithSpecifiedNumberOfInputs_ShouldCallCheckpointSameNumberOfTimes(params long[] availableIds)
        {
            // Arrange
            List<long> checkpointedIds = new List<long>();

            var fixtures = new ProcessorTestFixtures()
                .WithIds(availableIds)
                .WithAllProcessorCallsSuccessful()
                .WithLoggedCheckpointCalls(checkpointedIds);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var info = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            var expectedNumberOfCalls = availableIds.Length;
            Assert.AreEqual(expectedNumberOfCalls, checkpointedIds.Count);
        }

        [Test]
        public async Task ProcessAsync_WithSpecifiedNumberOfInputs_ShouldCheckpointExpectedIds()
        {
            // Arrange
            List<long> checkpointedIds = new List<long>();
            List<long> processedIds = new List<long>();

            var fixtures = new ProcessorTestFixtures()
                .WithIds(25, 50)
                .WithLoggedCalls(processedIds)
                .WithLoggedCheckpointCalls(checkpointedIds);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var info = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            var unexpectedCheckpoints = checkpointedIds.Except(processedIds).Count();
            const int expectedNumberOfUnexpectedCheckpoints = 0;

            Assert.AreEqual(expectedNumberOfUnexpectedCheckpoints, unexpectedCheckpoints);
        }

        [TestCase(0, 10)]
        [TestCase(1, 10)]
        [TestCase(9, 10)]
        [TestCase(10, 10)]
        [TestCase(11, 10)]
        public async Task ProcessAsync_WithMultipleBatches_ShouldProcessAllInputs(int totalInputSize, int batchSize)
        {
            // Arrange
            List<long> processedIds = new List<long>();

            var fixtures = new ProcessorTestFixtures()
                .WithIds(1, totalInputSize)
                .WithBatchSize(10)
                .WithLoggedCalls(processedIds);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            var expectedNumberOfCalls = totalInputSize;
            Assert.AreEqual(expectedNumberOfCalls, processedIds.Count);
        }

        [Test]
        public async Task ProcessAsync_WithSpecifiedStart_ShouldNotAskForLessThenStart()
        {
            // Arrange
            const int lastSuccessfulId = 5;

            var processedBatches = new List<BatchInfo>();

            var fixtures = new ProcessorTestFixtures()
                .WithLoggedIds(1, 10, processedBatches)
                .WithStartingAfterLastSuccess(lastSuccessfulId);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            const int expectedNumberOfBatches = 1;
            Assert.AreEqual(expectedNumberOfBatches, processedBatches.Count);

            Assert.AreEqual(lastSuccessfulId, processedBatches[0].StartAfterId);
        }

        [TestCase(0, 10, 1)]
        [TestCase(1, 10, 1)]
        [TestCase(9, 10, 1)]
        [TestCase(10, 10, 2)]
        [TestCase(11, 10, 2)]
        public async Task ProcessAsync_WithMultipleBatches_ShouldMakeExpectedNumberOfCallsToFetchIds(int totalInputSize, int batchSize, int expectedNumberOfIdProviderCalls)
        {
            // Arrange
            var processedBatches = new List<BatchInfo>();

            var fixtures = new ProcessorTestFixtures()
                .WithBatchSize(10)
                .WithAllProcessorCallsSuccessful()
                .WithLoggedIds(1, totalInputSize, processedBatches);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            Assert.AreEqual(expectedNumberOfIdProviderCalls, processedBatches.Count);
        }

        [TestCase(0, 10, 0)]
        [TestCase(1, 10, 1)]
        [TestCase(9, 10, 1)]
        [TestCase(10, 10, 1)]
        [TestCase(11, 10, 2)]
        public async Task ProcessAsync_WithMultipleBatches_ShouldSetProcessedBatchesCorrectly(int totalInputSize, int batchSize, int expectedProcessedBatches)
        {
            // Arrange
            var processedBatches = new List<BatchInfo>();

            var fixtures = new ProcessorTestFixtures()
                .WithBatchSize(10)
                .WithAllProcessorCallsSuccessful()
                .WithLoggedIds(1, totalInputSize, processedBatches);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo =  await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            Assert.AreEqual(expectedProcessedBatches, processInfo.BatchesProcessed);
        }

        [Test]
        public async Task ProcessAsync_AfterCompletion_ShouldSetProcessInfoCorrectly()
        {
            // Arrange
            const int OffSet = 300;
            const int numberOfs = 25;

            var fixtures = new ProcessorTestFixtures()
                .WithIds(OffSet, numberOfs)
                .WithAllProcessorCallsSuccessful();

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            const int expectedNumberOfBatches = 1;
            const int expectedMaximumId = OffSet + numberOfs - 1;

            Assert.AreEqual(expectedNumberOfBatches, processInfo.BatchesProcessed, $"{nameof(processInfo.BatchesProcessed)} is incorrect");
            Assert.AreEqual(expectedMaximumId, processInfo.LastProcessedId, $"{nameof(processInfo.LastProcessedId)} is incorrect");
            Assert.AreEqual(numberOfs, processInfo.IdsProcessed, $"{nameof(processInfo.IdsProcessed)} is incorrect");
            Assert.Less(processInfo.StartTime, processInfo.EndTime, $"{nameof(processInfo.StartTime)} should be less than {nameof(processInfo.EndTime)}");
        }

        [Test]
        public async Task ProcessAsync_WhenProviderParametersAreNotRespected_ShouldStillProcessInBatches()
        {
            // Arrange
            const int batchSize = 50;

            var processedBatches = new List<BatchInfo>();
            var processedIds = new List<long>();

            var sequence1 = Enumerable.Range(1, 100).Select(Convert.ToInt64);

            var fixtures = new ProcessorTestFixtures()
                .WithBatchSize(batchSize)
                .WithLoggedCalls(processedIds)
                .WithExactLoggedIds(sequence1, processedBatches);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            const int expectedProcessedBatches = 2;
            Assert.AreEqual(expectedProcessedBatches, processInfo.BatchesProcessed);

            const int firstBatchStartsAfter = 0;
            const int secondBatchStartsAfter = firstBatchStartsAfter + batchSize;
            Assert.AreEqual(firstBatchStartsAfter, processedBatches[0].StartAfterId);
            Assert.AreEqual(secondBatchStartsAfter, processedBatches[1].StartAfterId);

            const int expectedNumberOfDuplicateds = 0;
            var duplicatesProcessed = processedIds.GroupBy(i => i).Count(g => g.Count() > 1);
            Assert.AreEqual(expectedNumberOfDuplicateds, duplicatesProcessed);
        }

        [Test]
        public async Task ProcessAsync_WhenExceptionsAreThrownAndIgnored_ShouldRecordCorrectNumberOfExceptions()
        {
            // Arrange
            const int numberOfIds = 100;
            var fixtures = new ProcessorTestFixtures()
                .WithIds(1, numberOfIds)
                .WithAllProcessorCallsFaulting()
                .WithAllProcessingFaultsIgnored();

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            Assert.AreEqual(numberOfIds, processInfo.UnhandledExceptions.Count);
        }

        [Test]
        public async Task ProcessAsync_WhenExceptionsAreThrownAndIgnored_ShouldRecordExceptionMessages()
        {
            // Arrange
            const int numberOfIds = 5;
            var fixtures = new ProcessorTestFixtures()
                .WithIds(1, numberOfIds)
                .WithAllProcessorCallsFaulting()
                .WithAllProcessingFaultsIgnored();

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            for (int i = 0; i < numberOfIds; i++)
            {
                // exception messages are set to the ID that was being processed
                var expectedExceptionMessage = (i + 1).ToString(CultureInfo.InvariantCulture);
                Assert.AreEqual(expectedExceptionMessage, processInfo.UnhandledExceptions[i]);
            }
        }

        [Test]
        public async Task ProcessAsync_WhenExceptionsAreThrownAndHaultProcessing_ShouldSetStatusToTerminatedEarlyByExceptionHandler()
        {
            // Arrange
            var fixtures = new ProcessorTestFixtures()
                .WithIds(1, 100)
                .WithAllProcessorCallsFaulting()
                .WithAnyProcessingFaultTerminatingProcess();

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            const ProcessingState expectedStatus = ProcessingState.TerminatedByErrorHandler;
            Assert.AreEqual(expectedStatus, processInfo.State);
        }

        [Test]
        public async Task ProcessAsync_WhenNoExceptionThrownButProcessinsTerminatedEarly_ShouldSetStatusToTerminatedEarlyHandler()
        {
            // Arrange
            var fixtures = new ProcessorTestFixtures()
                .WithIds(1, 100)
                .WithProcessorCallsSuccessfulUntilSetPoint(5);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            const ProcessingState expectedStatus = ProcessingState.TerminatedEarly;
            Assert.AreEqual(expectedStatus, processInfo.State);
        }

        [Test]
        public async Task ProcessAsync_WhenAllIdsProcessedSuccessfully_ShouldSetStatusToSuccess()
        {
            // Arrange
            var fixtures = new ProcessorTestFixtures()
                .WithIds(1, 100)
                .WithAllProcessorCallsSuccessful();

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            const ProcessingState expectedStatus = ProcessingState.Success;
            Assert.AreEqual(expectedStatus, processInfo.State);
        }

        [Test]
        public async Task ProcessAsync_WhenExceptionsAreThrownAndHaultProcessing_ShouldOnlyAttemptToProcessOneId()
        {
            // Arrange
            var processedIds = new List<long>();

            var fixtures = new ProcessorTestFixtures()
                .WithIds(1, 100)
                .WithLoggedCalls(processedIds)
                .WithAllProcessorCallsFaulting()
                .WithAnyProcessingFaultTerminatingProcess();

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            const int expectedNumberOfIdsProcessed = 0;
            Assert.AreEqual(expectedNumberOfIdsProcessed, processedIds.Count);
        }

        [Test]
        public async Task ProcessAsync_WhenProcessorIndicatesTerminatePartWayThrough_ShouldSetStatusToTerminatedEarly()
        {
            // Arrange
            var fixtures = new ProcessorTestFixtures()
                .WithIds(1, 10)
                .WithProcessorCallsSuccessfulUntilSetPoint(5);

            var broker = new Broker(fixtures.IdProcessorConfiguration, fixtures.Checkpoint, fixtures.Log);

            // Act
            var processInfo = await broker.ProcessAsync(fixtures.IdProvider, fixtures.Processor);

            // Assert
            const int expectedNumberOfIdsProcessed = 4;
            Assert.AreEqual(expectedNumberOfIdsProcessed, processInfo.IdsProcessed);
        }
    }
}
