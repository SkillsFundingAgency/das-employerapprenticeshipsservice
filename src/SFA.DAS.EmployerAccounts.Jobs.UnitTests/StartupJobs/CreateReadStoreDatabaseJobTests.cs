using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
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
            return TestAsync(f => f.Run(), f => f.DocumentClient.Verify(c => c.CreateDatabaseIfNotExistsAsync(It.Is<Database>(d => d.Id == DocumentSettings.DatabaseName), null), Times.Once));
        }

        [Test]
        public Task Run_WhenRunningCreateReadStoreDatabaseJob_ThenShouldCreateReadStoreUsersCollectionWithPartitionAndUniqueKey()
        {
            return TestAsync(f => f.Run(), f => f.DocumentClient.Verify(c => c.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DocumentSettings.DatabaseName),
                It.Is<DocumentCollection>(d =>
                    d.Id == DocumentSettings.AccountUsersCollectionName &&
                    d.PartitionKey.Paths.Contains("/accountId") &&
                    d.UniqueKeyPolicy.UniqueKeys[0].Paths.Contains("/userRef")
                ), It.IsAny<RequestOptions>()), Times.Once));
        }

        [Test]
        public Task Run_WhenRunningCreateReadStoreDatabaseJob_ThenShouldCreateReadStoreUsersCollectionWithOfferThroughputSet()
        {
            return TestAsync(f => f.Run(), f => f.DocumentClient.Verify(c => c.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DocumentSettings.DatabaseName),
                It.IsAny<DocumentCollection>(), It.Is<RequestOptions>(p=>p.OfferThroughput == 1000)), Times.Once));
        }
    }

    public class CreateReadStoreDatabaseJobTestsFixture
    {
        public Mock<IDocumentClient> DocumentClient { get; set; }
        public Mock<ILogger<CreateReadStoreDatabaseJob>> Logger { get; set; }
        public CreateReadStoreDatabaseJob CreateReadStoreDatabaseJob { get; set; }

        public CreateReadStoreDatabaseJobTestsFixture()
        {
            DocumentClient = new Mock<IDocumentClient>();
            Logger = new Mock<ILogger<CreateReadStoreDatabaseJob>>();
            CreateReadStoreDatabaseJob = new CreateReadStoreDatabaseJob(DocumentClient.Object, Logger.Object);

            var resourceResponseDatabase = CreateResourceResponseWithStatusCode<Database>(HttpStatusCode.Created);

            DocumentClient.Setup(dc => dc.CreateDatabaseIfNotExistsAsync(It.Is<Database>(d =>
                        d.Id == "SFA.DAS.EmployerAccounts.ReadStore.Database"), null))
                .ReturnsAsync(resourceResponseDatabase);

            var resourceResponseDocumentCollection = CreateResourceResponseWithStatusCode<DocumentCollection>(HttpStatusCode.Created);

            DocumentClient.Setup(dc => dc.CreateDocumentCollectionIfNotExistsAsync(It.IsAny<Uri>(), It.IsAny<DocumentCollection>(), It.IsAny<RequestOptions>()))
                .ReturnsAsync(resourceResponseDocumentCollection);
        }

        public Task Run()
        {
            return CreateReadStoreDatabaseJob.Run();
        }

        /// <remarks>
        /// Microsoft are working on making ResourceResponse mockable, but until then we can use this monstrosity...
        /// See https://github.com/Azure/azure-cosmos-dotnet-v2/issues/393
        /// </remarks>
        public static ResourceResponse<TResource> CreateResourceResponseWithStatusCode<TResource>(HttpStatusCode statusCode)
            where TResource : Resource, new()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var documentServiceResponseType = typeof(IDocumentClient).Assembly.GetType("Microsoft.Azure.Documents.DocumentServiceResponse");

            var documentServiceResponse = Activator.CreateInstance(documentServiceResponseType, flags, null, new object[] { null, null, statusCode, null }, CultureInfo.InvariantCulture);

            return (ResourceResponse<TResource>)Activator.CreateInstance(typeof(ResourceResponse<TResource>), flags, null, new[] { documentServiceResponse, null }, CultureInfo.InvariantCulture);
        }
    }
}