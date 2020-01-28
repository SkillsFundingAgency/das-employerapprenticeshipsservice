using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
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
        public async Task Then_The_Draft_Expired_Funds_Are_Created_From_The_Command_And_Only_Include_Values_Upto_The_Supplied_Date(
            DraftExpireAccountFundsCommand message,
            IDictionary<CalendarPeriod, decimal> expiredFund,
            LevyFundsIn levyOne,
            LevyFundsIn levyTwo,
            PaymentFundsOut paymentOne,
            PaymentFundsOut paymentTwo,
            [Frozen] Mock<ILevyFundsInRepository> levyFundsInRepository,
            [Frozen] Mock<IPaymentFundsOutRepository> paymentFundsOutRepository,
            [Frozen] Mock<IExpiredFunds> expiredFunds,
            [Frozen] Mock<IExpiredFundsRepository> expiredFundsRepository,
            DraftExpireAccountFundsCommandHandler handler)
        {
            
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
            var paymentFundsOut = new List<PaymentFundsOut>{paymentOne,paymentTwo};
            paymentFundsOutRepository.Setup(x => x.GetPaymentFundsOut(message.AccountId)).ReturnsAsync(paymentFundsOut);

            expiredFunds.Setup(x => x.GetExpiringFunds(levyFundsIns.ToCalendarPeriodDictionary(),
                paymentFundsOut.ToCalendarPeriodDictionary(), It.IsAny<IDictionary<CalendarPeriod, decimal>>(),
                It.IsAny<int>())).Returns(expiredFund);
            
            await handler.Handle(message, new TestableMessageHandlerContext(new MessageMapper()));

            expiredFunds.Verify(x=>x.GetExpiringFunds(It.Is<IDictionary<CalendarPeriod, decimal>>(c=>c.Count().Equals(1)), It.Is<IDictionary<CalendarPeriod, decimal>>(c => c.Count().Equals(1)), It.IsAny<IDictionary<CalendarPeriod, decimal>>(),It.IsAny<int>()));
            expiredFundsRepository.Verify(x => x.CreateDraft(message.AccountId, It.Is<IEnumerable<ExpiredFund>>(c=>c.Count().Equals(expiredFund.Count)), It.IsAny<DateTime>()));
        }
    }
}
