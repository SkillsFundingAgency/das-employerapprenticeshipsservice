using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Worker.Infrastructure;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.UnitTests.Infrastructure
{
	[TestFixture]
	public class AzureWebJobHelperTests
	{
		[Test]
		public void Constructor_Valid_ShoudlNotFail()
		{
			var fixtures = new AzureWebJobHelperTestFixtures();

			var azureWebJobHelper = fixtures.CreateAzureWebJobHelper();

			Assert.Pass("Did not throw exception");
		}

		[Test]
		public void EnsureAllQueuesForTriggeredJobs_NoTriggeredJob_ShouldNotThrowException()
		{
			// arrange
			var fixtures = new AzureWebJobHelperTestFixtures();
			var azureWebJobHelper = fixtures.CreateAzureWebJobHelper();

			// act
			azureWebJobHelper.EnsureAllQueuesForTriggeredJobs();

			// Assert
			fixtures.AzureContainerRepositoryMock.Verify(acr => acr.EnsureQueueExistsAsync(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void EnsureAllQueuesForTriggeredJobs_OneTriggeredJob_ShouldVerifyOneQueue()
		{
			// arrange
			const string queueName = "testqueuename";
			var fixtures = new AzureWebJobHelperTestFixtures().AddTriggeredJob(queueName);
			var azureWebJobHelper = fixtures.CreateAzureWebJobHelper();

			// act
			azureWebJobHelper.EnsureAllQueuesForTriggeredJobs();

			// Assert
			fixtures.AzureContainerRepositoryMock.Verify(acr => acr.EnsureQueueExistsAsync(queueName), Times.Once);
		}

		[Test]
		public void EnsureAllQueuesForTriggeredJobs_TwoTriggeredJob_ShouldVerifyOneQueue()
		{
			// arrange
			const string queueName1 = "testqueuename1";
			const string queueName2 = "testqueuename2";
			var fixtures = new AzureWebJobHelperTestFixtures()
				.AddTriggeredJob(queueName1)
				.AddTriggeredJob(queueName2);

			var azureWebJobHelper = fixtures.CreateAzureWebJobHelper();

			// act
			azureWebJobHelper.EnsureAllQueuesForTriggeredJobs();

			// Assert
			fixtures.AzureContainerRepositoryMock.Verify(acr => acr.EnsureQueueExistsAsync(queueName1), Times.Once);
			fixtures.AzureContainerRepositoryMock.Verify(acr => acr.EnsureQueueExistsAsync(queueName2), Times.Once);
		}
	}

	class AzureWebJobHelperTestFixtures
	{
		public AzureWebJobHelperTestFixtures()
		{
			TriggeredJobRepositoryMock = new Mock<ITriggeredJobRepository>();
			AzureContainerRepositoryMock = new Mock<IAzureQueueClient>();
			LoggerMock = new Mock<ILog>();
			TriggeredJobs = new List<TriggeredJob<QueueTriggerAttribute>>();
			TriggeredJobRepositoryMock.Setup(tjr => tjr.GetQueuedTriggeredJobs()).Returns(TriggeredJobs);
		}

		public Mock<ITriggeredJobRepository> TriggeredJobRepositoryMock { get; set; }
		public ITriggeredJobRepository TriggeredJobRepository => TriggeredJobRepositoryMock.Object;

		public Mock<IAzureQueueClient> AzureContainerRepositoryMock { get; set; }
		public IAzureQueueClient AzureQueueClient => AzureContainerRepositoryMock.Object;

		public Mock<ILog> LoggerMock { get; set; }
		public ILog Logger => LoggerMock.Object;

		public List<TriggeredJob<QueueTriggerAttribute>> TriggeredJobs { get; }

		public AzureWebJobHelperTestFixtures AddTriggeredJob(string triggeredByQueue)
		{
			var triggerJob = new TriggeredJob<QueueTriggerAttribute>
			{
				ContainingClass = typeof(AzureWebJobHelperTestFixtures),
				InvokedMethod = typeof(AzureWebJobHelperTestFixtures).GetMethod(nameof(AddTriggeredJob)),
				Trigger = new QueueTriggerAttribute(triggeredByQueue)
			};

			TriggeredJobs.Add(triggerJob);

			return this;
		}

		public AzureWebJobHelper CreateAzureWebJobHelper()
		{
			return new AzureWebJobHelper(TriggeredJobRepository,  AzureQueueClient, Logger);
		}
	}
}
