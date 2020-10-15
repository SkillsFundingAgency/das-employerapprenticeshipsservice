using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Testing;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    [TestFixture, Parallelizable]
    public class ExpireAccountFundsCommandHandlerTests : FluentTest<ExpireAccountFundsCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingExpireAccountFundsCommand_ThenShouldCreateTheExpiredFundsRecords()
        {
            return RunAsync(f => f.Handle(),
                f => f.MockExpiredFundsRepository.Verify(x =>
                    x.Create(f.ExpectedAccountId, It.Is<IEnumerable<ExpiredFund>>(ex => f.AreExpiredFundsEqual(ex, f.ExpiredFunds)), f.Now), Times.Once));
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_No_Expiry_Returned_Then_Zero_Is_Added_For_Message_Date_Value(
            ExpireAccountFundsCommand message,
            [Frozen] Mock<ICurrentDateTime> currentDateTime,
            [Frozen] Mock<ILevyFundsInRepository> levyFundsInRepository,
            [Frozen] Mock<IPaymentFundsOutRepository> paymentFundsOutRepository,
            [Frozen] Mock<IExpiredFunds> expiredFunds,
            [Frozen] Mock<IExpiredFundsRepository> expiredFundsRepository,
            ExpireAccountFundsCommandHandler handler
        )
        {
            //Arrange
            currentDateTime.Setup(x => x.Now).Returns(DateTime.UtcNow);
            paymentFundsOutRepository.Setup(x => x.GetPaymentFundsOut(message.AccountId))
                .ReturnsAsync(new List<PaymentFundsOut>());
            levyFundsInRepository.Setup(x => x.GetLevyFundsIn(message.AccountId))
                .ReturnsAsync(new List<LevyFundsIn>());
            expiredFunds.Setup(x => x.GetExpiringFunds(
                It.IsAny<IDictionary<CalendarPeriod, decimal>>(),
                It.IsAny<IDictionary<CalendarPeriod, decimal>>(),
                It.IsAny<IDictionary<CalendarPeriod, decimal>>(),
                It.IsAny<int>())).Returns(new Dictionary<CalendarPeriod, decimal>());

            //Act
            await handler.Handle(message, new TestableMessageHandlerContext(new MessageMapper()));

            //Assert
            expiredFundsRepository.Verify(x => x.Create(message.AccountId,
                It.Is<IEnumerable<ExpiredFund>>(c => c.First().Amount.Equals(0)
                                                     && c.Count() == 1
                                                     && c.First().CalendarPeriodYear.Equals(DateTime.UtcNow.Year)
                                                     && c.First().CalendarPeriodMonth.Equals(DateTime.UtcNow.Month)
                ),
                It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public Task Handle_WhenHandlingExpireAccountFundsCommandAndAccountFundsAreExpired_ThenShouldPublishAccountFundsExpiredEvent()
        {
            return RunAsync(f => f.Handle(), f => f.VerifyAccountFundsExpiredEventPublished());
        }

        [Test]
        public Task Handle_WhenHandlingExpireAccountFundsCommandAndNoAccountFundsAreExpired_ThenShouldNotPublishAccountFundsExpiredEvent()
        {
            return RunAsync(f => f.ArrangeNoExpiringFunds(), f => f.Handle(),
                f => f.VerifyAccountFundsExpiredEventNotPublished());
        }

        [Test]
        //todo: better names
        public Task Handle_WhenHandlingExpireAccountFundsCommandAndNoAccountExpiredFundsAreReturned_ThenShouldNotPublishAccountFundsExpiredEvent()
        {
            return RunAsync(f => f.ArrangeNoExpiringFundsReturned(), f => f.Handle(),
                f => f.VerifyAccountFundsExpiredEventNotPublished());
        }
    }

    public class ExpireAccountFundsCommandHandlerTestsFixture
    {
        public DateTime Now { get; set; }
        public DateTime NextMonth { get; set; }
        public Mock<ICurrentDateTime> MockCurrentDateTime { get; set; }
        public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<ILevyFundsInRepository> MockLevyFundsInRepository { get; set; }
        public Mock<IPaymentFundsOutRepository> MockPaymentFundsOutRepository { get; set; }
        public Mock<IExpiredFunds> MockExpiredFunds { get; set; }
        public Mock<IExpiredFundsRepository> MockExpiredFundsRepository { get; set; }
        public Mock<ILog> MockLogger { get; set; }
        public EmployerFinanceConfiguration EmployerFinanceConfiguration { get; set; }

        public ExpireAccountFundsCommand Command { get; set; }
        public long ExpectedAccountId { get; set; }
        public List<LevyFundsIn> FundsIn { get; set; }
        public List<PaymentFundsOut> FundsOut { get; set; }
        public List<ExpiredFund> ExistingExpiredFunds { get; set; }
        public Dictionary<CalendarPeriod, decimal> ExpiringFunds { get; set; }
        public Dictionary<CalendarPeriod, decimal> ExpiredFunds { get; set; }
        public int FundsExpiryPeriod { get; set; }

        public IHandleMessages<ExpireAccountFundsCommand> Handler { get; set; }

        public ExpireAccountFundsCommandHandlerTestsFixture()
        {
            Now = DateTime.UtcNow;
            NextMonth = Now.AddMonths(1);
            MessageHandlerContext = new Mock<IMessageHandlerContext>();
            ExpectedAccountId = 112L;
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
            ExpiringFunds = new Dictionary<CalendarPeriod, decimal>
            {
                { new CalendarPeriod(2018, 03), 1000 },
                { new CalendarPeriod(2018, 04), 1000 },
                { new CalendarPeriod(2018, 05), 0 },
                { new CalendarPeriod(NextMonth.Year, NextMonth.Month), 1000 }
            };
            ExpiredFunds = new Dictionary<CalendarPeriod, decimal>
            {
                { new CalendarPeriod(2018, 03), 1000 },
                { new CalendarPeriod(2018, 04), 1000 },
                { new CalendarPeriod(2018, 05), 0 },
                { new CalendarPeriod(Now.Year, Now.Month), 0 }
            };

            MockCurrentDateTime = new Mock<ICurrentDateTime>();
            FundsExpiryPeriod = 18;

            MockLevyFundsInRepository = new Mock<ILevyFundsInRepository>();
            MockPaymentFundsOutRepository = new Mock<IPaymentFundsOutRepository>();
            MockExpiredFunds = new Mock<IExpiredFunds>();
            MockExpiredFundsRepository = new Mock<IExpiredFundsRepository>();
            MockLogger = new Mock<ILog>();
            EmployerFinanceConfiguration = new EmployerFinanceConfiguration{ FundsExpiryPeriod = FundsExpiryPeriod };

            MockCurrentDateTime.Setup(x => x.Now).Returns(Now);
            MockLevyFundsInRepository.Setup(x => x.GetLevyFundsIn(ExpectedAccountId)).ReturnsAsync(FundsIn);
            MockPaymentFundsOutRepository.Setup(x => x.GetPaymentFundsOut(ExpectedAccountId)).ReturnsAsync(FundsOut);
            MockExpiredFundsRepository.Setup(x => x.Get(ExpectedAccountId)).ReturnsAsync(ExistingExpiredFunds);
            MockExpiredFunds.Setup(x => x.GetExpiringFunds(
                It.Is<Dictionary<CalendarPeriod, decimal>>(fi => AreFundsInEqual(FundsIn, fi)),
                It.Is<Dictionary<CalendarPeriod, decimal>>(fo => AreFundsOutEqual(FundsOut, fo)),
                It.Is<Dictionary<CalendarPeriod, decimal>>(ex => AreExpiredFundsEqual(ExistingExpiredFunds, ex)),
                FundsExpiryPeriod)).Returns(ExpiringFunds);

            Handler = new ExpireAccountFundsCommandHandler(MockCurrentDateTime.Object, MockLevyFundsInRepository.Object, MockPaymentFundsOutRepository.Object, MockExpiredFunds.Object, MockExpiredFundsRepository.Object, MockLogger.Object, EmployerFinanceConfiguration);
        }

        public void ArrangeNoExpiringFunds()
        {
            MockExpiredFunds.Setup(x => x.GetExpiringFunds(
                It.Is<Dictionary<CalendarPeriod, decimal>>(fi => AreFundsInEqual(FundsIn, fi)),
                It.Is<Dictionary<CalendarPeriod, decimal>>(fo => AreFundsOutEqual(FundsOut, fo)),
                It.Is<Dictionary<CalendarPeriod, decimal>>(ex => AreExpiredFundsEqual(ExistingExpiredFunds, ex)),
                FundsExpiryPeriod)).Returns(new Dictionary<CalendarPeriod, decimal>
            {
                { new CalendarPeriod(2018, 03), 0m },
                { new CalendarPeriod(2018, 04), 0m }
            });
        }

        public void ArrangeNoExpiringFundsReturned()
        {
            MockExpiredFunds.Setup(x => x.GetExpiringFunds(
                It.Is<Dictionary<CalendarPeriod, decimal>>(fi => AreFundsInEqual(FundsIn, fi)),
                It.Is<Dictionary<CalendarPeriod, decimal>>(fo => AreFundsOutEqual(FundsOut, fo)),
                It.Is<Dictionary<CalendarPeriod, decimal>>(ex => AreExpiredFundsEqual(ExistingExpiredFunds, ex)),
                FundsExpiryPeriod)).Returns(new Dictionary<CalendarPeriod, decimal>());
        }

        public Task Handle()
        {
            return Handler.Handle(Command, MessageHandlerContext.Object);
        }

        public void VerifyAccountFundsExpiredEventPublished()
        {
            MessageHandlerContext.Verify(c => 
                c.Publish(It.Is<AccountFundsExpiredEvent>(e => e.AccountId == ExpectedAccountId), It.IsAny<PublishOptions>()),
                Times.Once);
        }

        public void VerifyAccountFundsExpiredEventNotPublished()
        {
            MessageHandlerContext.Verify(c =>  c.Publish(It.IsAny<AccountFundsExpiredEvent>(), It.IsAny<PublishOptions>()),
                Times.Never);
        }

        public bool AreFundsInEqual(IEnumerable<LevyFundsIn> fundsInList, IDictionary<CalendarPeriod, decimal> expiredFundsDictionary)
        {
            if (fundsInList.Count() != expiredFundsDictionary.Count)
            {
                return false;
            }

            foreach (var fund in fundsInList)
            {
                if (!expiredFundsDictionary.Any(x =>
                    x.Key.Year == fund.CalendarPeriodYear &&
                    x.Key.Month == fund.CalendarPeriodMonth &&
                    x.Value == fund.FundsIn))
                {
                    return false;
                }
            }

            return true;
        }

        public bool AreFundsOutEqual(IEnumerable<PaymentFundsOut> fundsOutList, IDictionary<CalendarPeriod, decimal> expiredFundsDictionary)
        {
            if (fundsOutList.Count() != expiredFundsDictionary.Count)
            {
                return false;
            }

            foreach (var fund in fundsOutList)
            {
                if (!expiredFundsDictionary.Any(x =>
                    x.Key.Year == fund.CalendarPeriodYear &&
                    x.Key.Month == fund.CalendarPeriodMonth &&
                    x.Value == -fund.FundsOut))
                {
                    return false;
                }
            }

            return true;
        }

        public bool AreExpiredFundsEqual(IEnumerable<ExpiredFund> expiredFundsEntityList, IDictionary<CalendarPeriod, decimal> expiredFundsDictionary)
        {
            if (expiredFundsEntityList.Count() != expiredFundsDictionary.Count)
            {
                return false;
            }

            foreach (var fund in expiredFundsEntityList)
            {
                if (!expiredFundsDictionary.Any(x =>
                    x.Key.Year == fund.CalendarPeriodYear &&
                    x.Key.Month == fund.CalendarPeriodMonth &&
                    x.Value == -fund.Amount))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
