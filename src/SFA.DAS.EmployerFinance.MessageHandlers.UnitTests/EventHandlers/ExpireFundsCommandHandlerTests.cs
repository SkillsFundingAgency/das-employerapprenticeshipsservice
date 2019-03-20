using System;
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
    }

    public class ExpireFundsCommandHandlerTestsFixture
    {
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<IFundsInRepository> MockFundsInRepository { get; set; }
        public Mock<IFundsOutRepository> MockFundsOutRepository { get; set; }
        public Mock<IExpiredFunds> MockExpiredFunds { get; set; }

        public ExpireFundsCommand Command { get; set; }
        public long ExpectedAccountId { get; set; }
        public List<LevyFundsIn> FundsIn { get; set; }
        public List<PaymentFundsOut> FundsOut { get; set; }
        public List<PaymentFundsOut> ExpiredFunds { get; set; }

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

            MockFundsInRepository = new Mock<IFundsInRepository>();
            MockFundsOutRepository = new Mock<IFundsOutRepository>();
            MockExpiredFunds = new Mock<IExpiredFunds>();

            MockFundsInRepository.Setup(x => x.GetFundsIn(ExpectedAccountId)).ReturnsAsync(FundsIn);
            MockFundsOutRepository.Setup(x => x.GetFundsOut(ExpectedAccountId)).ReturnsAsync(FundsOut);
            MockExpiredFunds.Setup(x => x.GetExpiringFunds(
                fi => fi[new CalendarPeriod(2018, 07)] == 12000 && fi[new CalendarPeriod(2018, 08)] == 15000,
                fo => fo[new CalendarPeriod(2018, 09)] == 10000 && fo[new CalendarPeriod(2018, 10)] == 10000),
                ex => ex[new CalendarPeriod(2018, )]);

            var periods = new Dictionary<CalendarPeriod, decimal>();
            var decimalPeriod = periods[new CalendarPeriod(1, 2)];

            Handler = new ExpireFundsCommandHandler(MockFundsInRepository.Object, MockFundsOutRepository.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Command, MessageHandlerContext.Object);
        }
    }
}
