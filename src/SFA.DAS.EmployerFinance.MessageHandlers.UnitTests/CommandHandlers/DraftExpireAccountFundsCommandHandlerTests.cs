using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.ExpiredFunds;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    [Parallelizable]
    public class DraftExpireAccountFundsCommandHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Then_The_Draft_Expired_Funds_Are_Created_From_The_Command(
            DraftExpireAccountFundsCommand message,
            IDictionary<CalendarPeriod, decimal> expiredFund,
            [Frozen] Mock<IExpiredFunds> expiredFunds,
            [Frozen] Mock<IExpiredFundsRepository> expiredFundsRepository,
            DraftExpireAccountFundsCommandHandler handler )
        {
            message.DateTo = null;
            expiredFunds.Setup(x => x.GetExpiringFunds(It.IsAny<IDictionary<CalendarPeriod, decimal>>(),
                It.IsAny<IDictionary<CalendarPeriod, decimal>>(), It.IsAny<IDictionary<CalendarPeriod, decimal>>(),
                It.IsAny<int>())).Returns(expiredFund);

            await handler.Handle(message,new TestableMessageHandlerContext(new MessageMapper()));

            expiredFundsRepository.Verify(x=>x.CreateDraft(message.AccountId, It.IsAny<IEnumerable<ExpiredFund>>(), It.IsAny<DateTime>()));
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Is_No_Expiry_Returned_Then_Zero_Is_Added_For_Message_Date_Value(
            DraftExpireAccountFundsCommand message,
            [Frozen] Mock<ILevyFundsInRepository> levyFundsInRepository,
            [Frozen] Mock<IPaymentFundsOutRepository> paymentFundsOutRepository,
            [Frozen] Mock<IExpiredFunds> expiredFunds,
            [Frozen] Mock<IExpiredFundsRepository> expiredFundsRepository,
            DraftExpireAccountFundsCommandHandler handler
            )
        {
            //Arrange
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
            expiredFundsRepository.Verify(x => x.CreateDraft(message.AccountId, 
                It.Is<IEnumerable<ExpiredFund>>(c => c.First().Amount.Equals(0) 
                                                     && c.Count()==1 
                                                     && c.First().CalendarPeriodYear.Equals(message.DateTo.Value.Year)
                                                     && c.First().CalendarPeriodMonth.Equals(message.DateTo.Value.Month)
                                                     ), 
                It.IsAny<DateTime>()), Times.Once);
        }


        [Test, MoqAutoData]
        public async Task Then_If_There_Is_No_Expiry_Returned_And_No_DateTo_Supplied_Then_Zero_Is_Added_For_Current_Year_And_Month(
            DraftExpireAccountFundsCommand message,
            [Frozen] Mock<ILevyFundsInRepository> levyFundsInRepository,
            [Frozen] Mock<IPaymentFundsOutRepository> paymentFundsOutRepository,
            [Frozen] Mock<IExpiredFunds> expiredFunds,
            [Frozen] Mock<IExpiredFundsRepository> expiredFundsRepository,
            DraftExpireAccountFundsCommandHandler handler
        )
        {
            //Arrange
            message.DateTo = null;
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
            expiredFundsRepository.Verify(x => x.CreateDraft(message.AccountId,
                It.Is<IEnumerable<ExpiredFund>>(c => c.First().Amount.Equals(0)
                                                     && c.Count() == 1
                                                     && c.First().CalendarPeriodYear.Equals(DateTime.UtcNow.Year)
                                                     && c.First().CalendarPeriodMonth.Equals(DateTime.UtcNow.Month)
                ),
                It.IsAny<DateTime>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Draft_Expired_Funds_Are_Created_From_The_Command_And_Only_Include_Values_Upto_The_Supplied_Date_And_Other_DraftExpiredFunds(
            DraftExpireAccountFundsCommand message,
            LevyFundsIn levyOne,
            LevyFundsIn levyTwo,
            PaymentFundsOut paymentOne,
            PaymentFundsOut paymentTwo,
            PaymentFundsOut paymentThree,
            int expiredFundsPeriod,
            decimal expiryAmount,
            Mock<EmployerFinanceConfiguration> configuration,
            [Frozen] Mock<ILevyFundsInRepository> levyFundsInRepository,
            [Frozen] Mock<IPaymentFundsOutRepository> paymentFundsOutRepository,
            [Frozen] Mock<IExpiredFunds> expiredFunds,
            [Frozen] Mock<IExpiredFundsRepository> expiredFundsRepository,
            [Frozen] Mock<ICurrentDateTime> currentDateTime,
            DraftExpireAccountFundsCommandHandler handler)
        {
            //Arrange
            currentDateTime.Setup(x => x.Now).Returns(DateTime.Now);
            expiredFundsRepository.Setup(x => x.Get(message.AccountId)).ReturnsAsync(new List<ExpiredFund>());
            expiredFundsRepository.Setup(x => x.GetDraft(message.AccountId)).ReturnsAsync(new List<ExpiredFund>());
            configuration.Setup(x => x.FundsExpiryPeriod).Returns(expiredFundsPeriod);
            message.DateTo = DateTime.Now.AddMonths(-1);
            levyOne.CalendarPeriodYear = message.DateTo.Value.Year;
            levyOne.CalendarPeriodMonth = message.DateTo.Value.Month;
            levyTwo.CalendarPeriodYear = DateTime.Now.Year;
            levyTwo.CalendarPeriodMonth = DateTime.Now.Month;
            var levyFundsIns = new List<LevyFundsIn> {levyOne, levyTwo};
            levyFundsInRepository.Setup(x => x.GetLevyFundsIn(message.AccountId))
                .ReturnsAsync(levyFundsIns);

            paymentOne.CalendarPeriodYear = message.DateTo.Value.Year;
            paymentOne.CalendarPeriodMonth = message.DateTo.Value.Month;
            paymentTwo.CalendarPeriodYear = DateTime.Now.Year;
            paymentTwo.CalendarPeriodMonth = DateTime.Now.Month;
            paymentThree.CalendarPeriodYear = DateTime.Now.AddMonths(-2).Year;
            paymentThree.CalendarPeriodMonth = DateTime.Now.AddMonths(-2).Month;
            var paymentFundsOut = new List<PaymentFundsOut>{paymentOne,paymentTwo,paymentThree};
            paymentFundsOutRepository.Setup(x => x.GetPaymentFundsOut(message.AccountId)).ReturnsAsync(paymentFundsOut);
            var expiredFund = new Dictionary<CalendarPeriod, decimal>
            {
                {new CalendarPeriod(message.DateTo.Value.Year, message.DateTo.Value.Month), expiryAmount}
            };
            expiredFunds.Setup(x => x.GetExpiringFunds(
                It.Is<IDictionary<CalendarPeriod, decimal>>(c => c.Count.Equals(2)),
                It.Is<IDictionary<CalendarPeriod, decimal>>(c => c.Count.Equals(1)),
                It.Is<IDictionary<CalendarPeriod, decimal>>(c => c.Count.Equals(0)),
                It.IsAny<int>()))
                .Returns(expiredFund);
            
            //Act
            await handler.Handle(message, new TestableMessageHandlerContext(new MessageMapper()));

            //Assert
            expiredFundsRepository.Verify(x => x.GetDraft(message.AccountId), Times.Once);
            expiredFundsRepository.Verify(x => x.CreateDraft(message.AccountId,
                It.Is<IEnumerable<ExpiredFund>>(c => 
                                                    c.First().Amount.Equals(expiryAmount*-1)
                                                 && c.Count() == 1
                                                 && c.First().CalendarPeriodYear.Equals(message.DateTo.Value.Year)
                                                 && c.First().CalendarPeriodMonth.Equals(message.DateTo.Value.Month)
                ),
                It.IsAny<DateTime>()), Times.Once);
        }
    }
}
