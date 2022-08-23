using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiRequests.HealthCheck;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiResponses.HealthCheck;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands
{
    [TestFixture]
    public class RunHealthCheckCommandHandlerTests : FluentTest<RunHealthCheckCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingARunHealthCheckCommand_ThenShouldAddAHealthCheck()
        {
            return RunAsync(f => f.Handle(), f => f.Db.Verify(d => d.HealthChecks.Add(It.IsAny<HealthCheck>())));
        }

        [Test]
        public Task Handle_WhenHandlingARunHealthCheckCommand_ThenShouldRequestAnEmployerAccountsApiHealthCheckResponse()
        {
            return RunAsync(f => f.Handle(), f => f.AccountsApiClient.Verify(c => c.Ping(), Times.Once));
        }

        [Test]
        public Task Handle_WhenHandlingARunHealthCheckCommand_ThenShouldRequestAnOuterApiHealthCheckResponse()
        {
            return RunAsync(f => f.Handle(), f => f.OuterApiClient.Verify(c => c.Get<PingResponse>(It.IsAny<PingRequest>()), Times.Once));
        }
    }

    public class RunHealthCheckCommandHandlerTestsFixture
    {
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public RunHealthCheckCommand RunHealthCheckCommand { get; set; }
        public IAsyncRequestHandler<RunHealthCheckCommand, Unit> Handler { get; set; }
        public Mock<IAccountApiClient> AccountsApiClient { get; set; }
        public Mock<IOuterApiClient> OuterApiClient { get; set; }
        public UnitOfWorkContext UnitOfWorkContext { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public RunHealthCheckCommandHandlerTestsFixture()
        {
            Db = new Mock<EmployerAccountsDbContext>();
            RunHealthCheckCommand = new RunHealthCheckCommand { UserRef = Guid.NewGuid() };
            AccountsApiClient = new Mock<IAccountApiClient>();
            OuterApiClient = new Mock<IOuterApiClient>();
            UnitOfWorkContext = new UnitOfWorkContext();
            CancellationToken = new CancellationToken();

            Db.Setup(d => d.HealthChecks.Add(It.IsAny<HealthCheck>()));
            AccountsApiClient.Setup(c => c.Ping()).Returns(Task.CompletedTask);

            Handler = new RunHealthCheckCommandHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object), AccountsApiClient.Object, OuterApiClient.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(RunHealthCheckCommand);
        }
    }
}