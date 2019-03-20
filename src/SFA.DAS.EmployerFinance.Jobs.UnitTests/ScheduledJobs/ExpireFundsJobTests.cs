using System;
using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Jobs.ScheduledJobs;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.Jobs.UnitTests.ScheduledJobs
{
    [TestFixture]
    [Parallelizable]
    public class ExpireFundsJobTests : FluentTest<ExpireFundsJobTestsFixture>
    {
        [Test]
        public Task Run_WhenRunningJob_ThenShouldSendCommand()
        {
            return RunAsync(f => f.Run(), f => f.MessageSession.Verify(s => s.Send(It.Is<ExpireFundsCommand>(c => c.Year == f.Now.Year && c.Month == f.Now.Month), It.IsAny<SendOptions>())));
        }
    }

    public class ExpireFundsJobTestsFixture
    {
        public DateTime Now { get; set; }
        public Mock<ICurrentDateTime> CurrentDateTime { get; set; }
        public Mock<IMessageSession> MessageSession { get; set; }
        public ExpireFundsJob Job { get; set; }

        public ExpireFundsJobTestsFixture()
        {
            Now = DateTime.UtcNow;
            CurrentDateTime = new Mock<ICurrentDateTime>();
            MessageSession = new Mock<IMessageSession>();

            CurrentDateTime.Setup(d => d.Now).Returns(Now);

            Job = new ExpireFundsJob(CurrentDateTime.Object, MessageSession.Object);
        }

        public Task Run()
        {
            return Job.Run(null, null);
        }
    }
}