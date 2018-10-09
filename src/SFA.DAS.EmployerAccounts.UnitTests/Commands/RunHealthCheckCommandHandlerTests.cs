using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork;

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
            return RunAsync(f => f.Handle(), f => f.EmployerAccountsApiClient.Verify(c => c.HealthCheck(), Times.Once));
        }
    }

    public class RunHealthCheckCommandHandlerTestsFixture
    {
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public RunHealthCheckCommand RunHealthCheckCommand { get; set; }
        public IAsyncRequestHandler<RunHealthCheckCommand, Unit> Handler { get; set; }
        public Mock<IEmployerAccountsApiClient> EmployerAccountsApiClient { get; set; }
        public UnitOfWorkContext UnitOfWorkContext { get; set; }

        public RunHealthCheckCommandHandlerTestsFixture()
        {
            Db = new Mock<EmployerAccountsDbContext>();
            RunHealthCheckCommand = new RunHealthCheckCommand { UserRef = Guid.NewGuid() };
            EmployerAccountsApiClient = new Mock<IEmployerAccountsApiClient>();
            UnitOfWorkContext = new UnitOfWorkContext();

            Db.Setup(d => d.HealthChecks.Add(It.IsAny<HealthCheck>()));
            EmployerAccountsApiClient.Setup(c => c.HealthCheck()).Returns(Task.CompletedTask);

            Handler = new RunHealthCheckCommandHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object), EmployerAccountsApiClient.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(RunHealthCheckCommand);
        }
    }
}