using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.UnitTests.Builders;
using SFA.DAS.Testing;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    public class HealthCheckEventHandlerTests : FluentTest<HealthCheckEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAHealthCheckEvent_ThenShouldUpdateHealthCheck()
        {
            return RunAsync(f => f.Handle(), f => f.HealthChecks[1].ReceivedEvent.Should().HaveValue());
        }
    }

    public class HealthCheckEventHandlerTestsFixture
    {
        public IHandleMessages<HealthCheckEvent> Handler { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public List<HealthCheck> HealthChecks { get; set; }

        public HealthCheckEventHandlerTestsFixture()
        {
            Db = new Mock<EmployerAccountsDbContext>();

            HealthChecks = new List<HealthCheck>
            {
                new HealthCheckBuilder().WithId(1).Build(),
                new HealthCheckBuilder().WithId(2).Build()
            };

            Db.Setup(d => d.HealthChecks).Returns(new DbSetStub<HealthCheck>(HealthChecks));

            Handler = new HealthCheckEventHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object));
        }

        public Task Handle()
        {
            return Handler.Handle(new HealthCheckEvent { Id = HealthChecks[1].Id }, null);
        }
    }
}