using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Commands
{
    [TestFixture]
    [Parallelizable]
    internal class PingCommandHandlerTests : FluentTest<PingCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenDatabasePingSucceeds_ThenShouldNotThrowException()
        {
            return TestExceptionAsync(
                f => f.Handle(),
                (f, r) => r.ShouldNotThrow<Exception>());
        }

        [Test]
        public Task Handle_WhenDatabasePingFails_ThenShouldThrowException()
        {
            return TestExceptionAsync(
                f => f.SetPingFailure(), 
                f => f.Handle(),
                (f, r) => r.ShouldThrow<Exception>().WithMessage("Read store database ping failed"));
        }
    }

    internal class PingCommandHandlerTestsFixture
    {
        public PingCommand Command { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Mock<IDocumentClient> DocumentClient { get; set; }
        public IReadStoreRequestHandler<PingCommand, Unit> Handler { get; set; }
        public List<Database> Databases { get; set; }

        public PingCommandHandlerTestsFixture()
        {
            Command = new PingCommand();
            CancellationToken = new CancellationToken();
            DocumentClient = new Mock<IDocumentClient>();
            Handler = new PingCommandHandler(DocumentClient.Object);
            Databases = new List<Database> { new Database { Id = DocumentSettings.DatabaseName }, new Database() };

            DocumentClient.Setup(c => c.CreateDatabaseQuery(null)).Returns(Databases.AsQueryable().OrderBy(d => d.Id));
        }

        public Task Handle()
        {
            return Handler.Handle(Command, CancellationToken);
        }

        public PingCommandHandlerTestsFixture SetPingFailure()
        {
            Databases.Clear();

            return this;
        }
    }
}