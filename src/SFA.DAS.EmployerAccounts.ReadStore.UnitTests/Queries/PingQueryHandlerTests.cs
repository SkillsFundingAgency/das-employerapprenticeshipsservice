using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Azure.Documents;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Queries
{
    [TestFixture]
    [Parallelizable]
    internal class PingQueryHandlerTests : FluentTest<PingQueryHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenDatabasePingSucceeds_ThenShouldNotThrowException()
        {
            return TestExceptionAsync(
                f => f.Handle(),
                (f, r) => r.Should().NotThrowAsync<Exception>());
        }

        [Test]
        public Task Handle_WhenDatabasePingFails_ThenShouldThrowException()
        {
            return TestExceptionAsync(
                f => f.SetPingFailure(), 
                f => f.Handle(),
                (f, r) => r.Should().ThrowAsync<Exception>().WithMessage("Read store database ping failed"));
        }
    }

    internal class PingQueryHandlerTestsFixture
    {
        public PingQuery Query { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Mock<IDocumentClient> DocumentClient { get; set; }
        public IRequestHandler<PingQuery, Unit> Handler { get; set; }
        public List<Database> Databases { get; set; }

        public PingQueryHandlerTestsFixture()
        {
            Query = new PingQuery();
            CancellationToken = new CancellationToken();
            DocumentClient = new Mock<IDocumentClient>();
            Handler = new PingQueryHandler(DocumentClient.Object);
            Databases = new List<Database> { new Database { Id = DocumentSettings.DatabaseName }, new Database() };

            DocumentClient.Setup(c => c.CreateDatabaseQuery(null)).Returns(Databases.AsQueryable().OrderBy(d => d.Id));
        }

        public Task Handle()
        {
            return Handler.Handle(Query, CancellationToken);
        }

        public PingQueryHandlerTestsFixture SetPingFailure()
        {
            Databases.Clear();

            return this;
        }
    }
}