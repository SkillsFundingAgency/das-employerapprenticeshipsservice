using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    public class ExpireAccountFundsCommandHandlerTests : FluentTest<ExpireAccountFundsCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingExpireAccountFundsCommand_ThenShouldGetTheFundsIn()
        {
            return RunAsync(f => f.Handle(),
                f => f.MockLevyFundsInRepository.Verify(r => r.GetLevyFundsIn(f.ExpectedAccountId)));
        }

        [Test]
        public Task Handle_WhenHandlingExpireAccountFundsCommand_ThenShouldGetTheFundsOut()
        {
            return RunAsync(f => f.Handle(),
                f => f.MockPaymentFundsOutRepository.Verify(r => r.GetPaymentFundsOut(f.ExpectedAccountId)));
        }

        [Test]
        public Task Handle_WhenHandlingExpireAccountFundsCommand_ThenShouldGetTheExistingExpiredFunds()
        {
            return RunAsync(f => f.Handle(), f => f.MockExpiredFundsRepository.Verify(x => x.Get(f.ExpectedAccountId)));
        }

        [Test]
        public Task Handle_WhenHandlingExpireAccountFundsCommand_ThenShouldCallTheExpiredFundsAlgorithm()
        {
            return RunAsync(f => f.Handle(), f => f.MockExpiredFunds.Verify(x => x.GetExpiringFunds(
                It.Is<Dictionary<CalendarPeriod, decimal>>(fi => ExpiredFundsComparisonHelper.AreFundsInEqual(f.FundsIn, fi)),
                It.Is<Dictionary<CalendarPeriod, decimal>>(fo => ExpiredFundsComparisonHelper.AreFundsOutEqual(f.FundsOut, fo)),
                It.Is<Dictionary<CalendarPeriod, decimal>>(ex => ExpiredFundsComparisonHelper.AreExpiredFundsEqual(f.ExistingExpiredFunds, ex)),
                24)));
        }

        [Test]
        public Task Handle_WhenHandlingExpireAccountFundsCommand_ThenShouldCreateTheExpiredFundsRecords()
        {
            return RunAsync(f => f.Handle(),
                f => f.MockExpiredFundsRepository.Verify(x =>
                    x.Create(f.ExpectedAccountId, It.Is<IEnumerable<ExpiredFund>>(ex => ExpiredFundsComparisonHelper.AreExpiredFundsEqual(ex, f.ExpectedExpiredFunds)))));
        }
    }

    public class ExpireAccountFundsCommandHandlerTestsFixture
    {
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<ILevyFundsInRepository> MockLevyFundsInRepository { get; set; }
        public Mock<IPaymentFundsOutRepository> MockPaymentFundsOutRepository { get; set; }
        public Mock<IExpiredFunds> MockExpiredFunds { get; set; }
        public Mock<IExpiredFundsRepository> MockExpiredFundsRepository { get; set; }

        public ExpireAccountFundsCommand Command { get; set; }
        public long ExpectedAccountId { get; set; }
        public List<LevyFundsIn> FundsIn { get; set; }
        public List<PaymentFundsOut> FundsOut { get; set; }
        public List<ExpiredFund> ExistingExpiredFunds { get; set; }
        public Dictionary<CalendarPeriod, decimal> ExpectedExpiredFunds { get; set; }

        public IHandleMessages<ExpireAccountFundsCommand> Handler { get; set; }

        public ExpireAccountFundsCommandHandlerTestsFixture()
        {
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            ExpectedAccountId = 112;
            Command = new ExpireAccountFundsCommand{ AccountId = ExpectedAccountId };
            FundsIn = new List<LevyFundsIn>
            {
                new LevyFundsIn{ CalendarPeriodYear = 2018, CalendarPeriodMonth = 07, FundsIn = 12000 },
                new LevyFundsIn{ CalendarPeriodYear = 2018, CalendarPeriodMonth = 08, FundsIn = 15000 }
            };
            FundsOut = new List<PaymentFundsOut>
            {
                new PaymentFundsOut{  CalendarPeriodYear = 2018, CalendarPeriodMonth = 09, FundsOut = 10000 },
                new PaymentFundsOut{  CalendarPeriodYear = 2018, CalendarPeriodMonth = 10, FundsOut = 10000 }
            };
            ExistingExpiredFunds = new List<ExpiredFund>
            {
                new ExpiredFund{ CalendarPeriodYear = 2018, CalendarPeriodMonth = 01, Amount = 2000 },
                new ExpiredFund{ CalendarPeriodYear = 2018, CalendarPeriodMonth = 02, Amount = 2000 }
            };
            ExpectedExpiredFunds = new Dictionary<CalendarPeriod, decimal>
            {
                { new CalendarPeriod(2018, 03), 1000 },
                { new CalendarPeriod(2018, 04), 1000 }
            };

            MockLevyFundsInRepository = new Mock<ILevyFundsInRepository>();
            MockPaymentFundsOutRepository = new Mock<IPaymentFundsOutRepository>();
            MockExpiredFunds = new Mock<IExpiredFunds>();
            MockExpiredFundsRepository = new Mock<IExpiredFundsRepository>();

            MockLevyFundsInRepository.Setup(x => x.GetLevyFundsIn(ExpectedAccountId)).ReturnsAsync(FundsIn);
            MockPaymentFundsOutRepository.Setup(x => x.GetPaymentFundsOut(ExpectedAccountId)).ReturnsAsync(FundsOut);
            MockExpiredFundsRepository.Setup(x => x.Get(ExpectedAccountId)).ReturnsAsync(ExistingExpiredFunds);
            MockExpiredFunds.Setup(x => x.GetExpiringFunds(
                It.IsAny<Dictionary<CalendarPeriod, decimal>>(),
                It.IsAny<Dictionary<CalendarPeriod, decimal>>(),
                It.IsAny<Dictionary<CalendarPeriod, decimal>>(),
                It.IsAny<int>())).Returns(ExpectedExpiredFunds);

            Handler = new ExpireAccountFundsCommandHandler(MockLevyFundsInRepository.Object, MockPaymentFundsOutRepository.Object, MockExpiredFunds.Object, MockExpiredFundsRepository.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Command, MessageHandlerContext.Object);
        }
    }
}
