using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Api.Client;
using SFA.DAS.EmployerFinance.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands
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
        public Task Handle_WhenHandlingARunHealthCheckCommand_ThenShouldRequestAnEmployerFinanceApiHealthCheckResponse()
        {
            return RunAsync(f => f.Handle(), f => f.EmployerFinanceApiClient.Verify(c => c.HealthCheck(), Times.Once));
        }
    }

    public class RunHealthCheckCommandHandlerTestsFixture
    {
        public Mock<EmployerFinanceDbContext> Db { get; set; }
        public RunHealthCheckCommand RunHealthCheckCommand { get; set; }
        public IAsyncRequestHandler<RunHealthCheckCommand, Unit> Handler { get; set; }
        public Mock<IEmployerFinanceApiClient> EmployerFinanceApiClient { get; set; }
        public UnitOfWorkContext UnitOfWorkContext { get; set; }

        public RunHealthCheckCommandHandlerTestsFixture()
        {
            Db = new Mock<EmployerFinanceDbContext>();
            RunHealthCheckCommand = new RunHealthCheckCommand { UserRef = Guid.NewGuid() };
            EmployerFinanceApiClient = new Mock<IEmployerFinanceApiClient>();
            UnitOfWorkContext = new UnitOfWorkContext();

            Db.Setup(d => d.HealthChecks.Add(It.IsAny<HealthCheck>()));

            Handler = new RunHealthCheckCommandHandler(new Lazy<EmployerFinanceDbContext>(() => Db.Object), EmployerFinanceApiClient.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(RunHealthCheckCommand);
        }
    }
}