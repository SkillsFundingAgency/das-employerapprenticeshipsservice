using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Jobs.StartupJobs;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.Jobs.UnitTests.StartupJobs
{
    [TestFixture]
    [Parallelizable]
    public class CreateReadStoreDatabaseJobTests : FluentTest<CreateReadStoreDatabaseJobTestsFixture>
    {
        [Test]
        public Task Run_WhenRunningCreateReadStoreDatabaseJob_ThenShouldCreateReadStoreDatabase()
        {
            return RunAsync(f => f.Run(), f => f.DocumentClient.Verify(c => c.CreateDatabaseIfNotExistsAsync(It.Is<Database>(d => d.Id == DocumentSettings.DatabaseName), null), Times.Once));
        }

        [Test]
        public Task Run_WhenRunningCreateReadStoreDatabaseJob_ThenShouldCreateReadStoreUsersCollection()
        {
            return RunAsync(f => f.Run(), f => f.DocumentClient.Verify(c => c.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DocumentSettings.DatabaseName),
                It.Is<DocumentCollection>(d =>
                    d.Id == DocumentSettings.UsersCollectionName &&
                    d.PartitionKey.Paths.Contains("/userRef") &&
                    d.UniqueKeyPolicy.UniqueKeys[0].Paths.Contains("/accountId")
                ), null), Times.Once));
        }
    }

    public class CreateReadStoreDatabaseJobTestsFixture
    {
        public Mock<IDocumentClient> DocumentClient { get; set; }
        public CreateReadStoreDatabaseJob CreateReadStoreDatabaseJob { get; set; }

        public CreateReadStoreDatabaseJobTestsFixture()
        {
            DocumentClient = new Mock<IDocumentClient>();
            CreateReadStoreDatabaseJob = new CreateReadStoreDatabaseJob(DocumentClient.Object);
        }

        public Task Run()
        {
            return CreateReadStoreDatabaseJob.Run();
        }
    }
}