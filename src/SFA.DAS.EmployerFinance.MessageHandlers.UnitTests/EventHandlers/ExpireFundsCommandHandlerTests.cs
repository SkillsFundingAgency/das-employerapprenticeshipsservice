using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ExpireFundsCommandHandlerTests : FluentTest<ExpireFundsCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingExpireFundsCommand_ThenShouldGetTheFundsIn()
        {
            return RunAsync(f => f.Handle(),
                f => f.MockFundsInRepository.Verify(r => r.GetFundsIn(f.ExpectedAccountId)));
        }

        [Test]
        public Task Handle_WhenHandlingExpireFundsCommand_ThenShouldGetTheFundsOut()
        {
            return RunAsync(f => f.Handle(),
                f => f.MockFundsOutRepository.Verify(r => r.GetFundsOut(f.ExpectedAccountId)));
        }

        [Test]
        public Task Handle_WhenHandlingExpireFundsCommand_ThenShouldGetTheExistingExpiredFunds()
        {
            return RunAsync(f => f.Handle(), f => f.MockExpiredFundsRepository.Verify(x => x.Get(f.ExpectedAccountId)));
        }

        [Test]
        public Task Handle_WhenHandlingExpireFundsCommand_ThenShouldCallTheExpiredFundsAlgorithm()
        {
            //return RunAsync(f => f.Handle(), f => f.MockExpiredFunds.Verify(x => x.GetExpiringFunds(
            //    It.Is<Dictionary<CalendarPeriod, decimal>>(fi => fi.Any(x => x.Key.CompareTo()) && fi[new CalendarPeriod(2018, 08)] == 15000),
            //    //It.Is<Dictionary<CalendarPeriod, decimal>>(fi => fi.ContainsKey(new CalendarPeriod(2018, 07)) && fi[new CalendarPeriod(2018, 07)] == 12000 && fi[new CalendarPeriod(2018, 08)] == 15000),
            //    It.IsAny<Dictionary<CalendarPeriod, decimal>>(),//It.Is<IDictionary<CalendarPeriod, decimal>>(fo => fo.ContainsKey(new CalendarPeriod(2018, 09)) && fo[new CalendarPeriod(2018, 09)] == 10000 && fo[new CalendarPeriod(2018, 10)] == 10000),
            //    It.IsAny<Dictionary<CalendarPeriod, decimal>>(),//It.Is<IDictionary<CalendarPeriod, decimal>>(ex => ex.ContainsKey(new CalendarPeriod(2018, 01)) && ex[new CalendarPeriod(2018, 01)] == 2000 && ex[new CalendarPeriod(2018, 02)] == 2000),
            //    24)));
            return Task.CompletedTask;
        }
    }

    public class ExpireFundsCommandHandlerTestsFixture
    {
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<IFundsInRepository> MockFundsInRepository { get; set; }
        public Mock<IFundsOutRepository> MockFundsOutRepository { get; set; }
        public Mock<IExpiredFunds> MockExpiredFunds { get; set; }
        public Mock<IExpiredFundsRepository> MockExpiredFundsRepository { get; set; }

        public ExpireFundsCommand Command { get; set; }
        public long ExpectedAccountId { get; set; }
        public List<LevyFundsIn> FundsIn { get; set; }
        public List<PaymentFundsOut> FundsOut { get; set; }
        public List<ExpiredFund> ExistingExpiredFunds { get; set; }
        public List<ExpiredFund> ExpectedExpiredFunds { get; set; }

        public IHandleMessages<ExpireFundsCommand> Handler { get; set; }

        public ExpireFundsCommandHandlerTestsFixture()
        {
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            ExpectedAccountId = 112;
            Command = new ExpireFundsCommand{ AccountId = ExpectedAccountId };
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
            ExpectedExpiredFunds = new List<ExpiredFund>
            {
                new ExpiredFund{ CalendarPeriodYear = 2018, CalendarPeriodMonth = 03, Amount = 1000 },
                new ExpiredFund{ CalendarPeriodYear = 2018, CalendarPeriodMonth = 04, Amount = 1000 }
            };

            MockFundsInRepository = new Mock<IFundsInRepository>();
            MockFundsOutRepository = new Mock<IFundsOutRepository>();
            MockExpiredFunds = new Mock<IExpiredFunds>();
            MockExpiredFundsRepository = new Mock<IExpiredFundsRepository>();

            MockFundsInRepository.Setup(x => x.GetFundsIn(ExpectedAccountId)).ReturnsAsync(FundsIn);
            MockFundsOutRepository.Setup(x => x.GetFundsOut(ExpectedAccountId)).ReturnsAsync(FundsOut);
            MockExpiredFundsRepository.Setup(x => x.Get(ExpectedAccountId)).ReturnsAsync(ExistingExpiredFunds);
            MockExpiredFunds.Setup(x => x.GetExpiringFunds(
                 It.Is<Dictionary<CalendarPeriod, decimal>>(fi => fi.ContainsKey(new CalendarPeriod(2018, 07)) && fi[new CalendarPeriod(2018, 07)] == 12000 && fi[new CalendarPeriod(2018, 08)] == 15000),
                It.Is<Dictionary<CalendarPeriod, decimal>>(fo => fo.ContainsKey(new CalendarPeriod(2018, 09)) && fo[new CalendarPeriod(2018, 09)] == 10000 && fo[new CalendarPeriod(2018, 10)] == 10000),
                 It.Is<Dictionary<CalendarPeriod, decimal>>(ex => ex.ContainsKey(new CalendarPeriod(2018, 01)) && ex[new CalendarPeriod(2018, 02)] == 2000),
                 24)).Returns(new Dictionary<CalendarPeriod, decimal>{ { new CalendarPeriod(2018, 03), 1000 }, { new CalendarPeriod(2018, 04), 1000 } });
            

            //var periods = new Dictionary<CalendarPeriod, decimal>();
            //var decimalPeriod = periods[new CalendarPeriod(1, 2)];

            Handler = new ExpireFundsCommandHandler(MockFundsInRepository.Object, MockFundsOutRepository.Object, MockExpiredFunds.Object, MockExpiredFundsRepository.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Command, MessageHandlerContext.Object);
        }
    }
}
