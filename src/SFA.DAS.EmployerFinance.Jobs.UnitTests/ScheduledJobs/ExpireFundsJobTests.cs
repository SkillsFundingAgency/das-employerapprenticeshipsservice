using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Jobs.ScheduledJobs;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.Jobs.UnitTests.ScheduledJobs
{
    [TestFixture]
    [Parallelizable]
    public class ExpireFundsJobTests : FluentTest<ExpireFundsJobTestsFixture>
    {
        [Test]
        [TestCase(0, Description = "No accounts")]
        [TestCase(1, Description = "Single account")]
        [TestCase(99, Description = "Multiple accounts")]
        public Task Run_WhenRunningJob_ThenShouldSendCommand(int numberOfAccounts)
        {
            return RunAsync(
                f => f.SetupAccounts(numberOfAccounts),
                f => f.Run(), 
                f => f.MessageSession.Verify(s => s.Send(It.IsAny<ExpireAccountFundsCommand>(), It.IsAny<SendOptions>()), Times.Exactly(numberOfAccounts)));
        }
    }

    public class ExpireFundsJobTestsFixture
    {
        public Mock<IMessageSession> MessageSession { get; set; }
        public Mock<ICurrentDateTime> CurrentDateTime { get; set; }
        public Mock<IEmployerAccountRepository> EmployerAccountRepository;
        public ExpireFundsJob Job { get; set; }
        private readonly IFixture Fixture;

        public ExpireFundsJobTestsFixture()
        {
            Fixture = new Fixture();
            
            MessageSession = new Mock<IMessageSession>();
            CurrentDateTime = new Mock<ICurrentDateTime>();
            EmployerAccountRepository = new Mock<IEmployerAccountRepository>();

            Job = new ExpireFundsJob(MessageSession.Object, CurrentDateTime.Object, EmployerAccountRepository.Object);
        }

        public Task Run()
        {
            return Job.Run(null, Mock.Of<ILogger>());
        }

        internal void SetupAccounts(int numberOfAccounts)
        {
            var accounts = Fixture
                .Build<Account>().Without(acc => acc.AccountLegalEntities)
                .CreateMany(numberOfAccounts);

            EmployerAccountRepository.Setup(x => x.GetAll()).ReturnsAsync(accounts.ToList());
        }
    }
}