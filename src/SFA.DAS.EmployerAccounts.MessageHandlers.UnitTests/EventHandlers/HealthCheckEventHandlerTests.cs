using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    public class HealthCheckEventHandlerTests : Testing.FluentTest<HealthCheckEventHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAHealthCheckEvent_ThenShouldUpdateHealthCheck()
        {
            return TestAsync(f => f.Handle(), f => f.HealthChecks[1].ReceivedEvent.Should().HaveValue());
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
                ObjectActivator.CreateInstance<HealthCheck>().Set(x=>x.Id, 1),
                ObjectActivator.CreateInstance<HealthCheck>().Set(x=>x.Id, 2)
            };

            Db.Setup(d => d.HealthChecks).Returns(HealthChecks.AsQueryable().BuildMockDbSet().Object);

            Handler = new HealthCheckEventHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object));
        }

        public Task Handle()
        {
            return Handler.Handle(new HealthCheckEvent { Id = HealthChecks[1].Id }, null);
        }
    }
}