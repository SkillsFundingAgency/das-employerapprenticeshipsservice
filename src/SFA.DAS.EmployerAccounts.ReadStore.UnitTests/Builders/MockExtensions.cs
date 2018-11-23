using System.Collections.Generic;
using Moq;
using SFA.DAS.CosmosDb;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Builders
{
    public static class MockExtensions
    {
        public static void SetupInMemoryCollection<T, TDocument>(this Mock<T> mock, IEnumerable<TDocument> list) where T : class, IDocumentRepository<TDocument> where TDocument : class
        {
            var documentQuery = new FakeDocumentQuery<TDocument>(list);
            mock.Setup(x => x.CreateQuery(null)).Returns(documentQuery);
        }
    }


}
