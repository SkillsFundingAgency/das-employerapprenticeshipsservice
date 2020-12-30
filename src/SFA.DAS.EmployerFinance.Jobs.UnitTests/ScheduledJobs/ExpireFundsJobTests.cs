﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        public Task Run_WhenRunningJob_ThenShouldSendCommand_Hotfix_ShouldNotSendCommand()
        {
            return RunAsync(f => f.Run(), f => f.MessageSession.Verify(s => s.Send(It.IsAny<ExpireFundsCommand>(), It.IsAny<SendOptions>()), Times.Once));
        }
    }

    public class ExpireFundsJobTestsFixture
    {
        public Mock<IMessageSession> MessageSession { get; set; }
        public Mock<ILogger> Logger { get; set; }
        public ExpireFundsJob Job { get; set; }

        public ExpireFundsJobTestsFixture()
        {
            MessageSession = new Mock<IMessageSession>();
            Logger = new Mock<ILogger>();

            Job = new ExpireFundsJob(MessageSession.Object, Logger.Object);
        }

        public Task Run()
        {
            return Job.Run(null);
        }
    }
}