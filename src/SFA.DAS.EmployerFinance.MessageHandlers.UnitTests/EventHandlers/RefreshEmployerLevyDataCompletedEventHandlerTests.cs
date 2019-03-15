using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    public class
        RefreshEmployerLevyDataCompletedEventHandlerTests : FluentTest<RefreshEmployerLevyDataCompletedEventHandlerTestsFixture>
    {
        [Test]
        public Task WhenMessageIsHandled_RefreshEmployerLevyDataCompletedMessageIsPublished()
        {
            var timestamp = DateTime.UtcNow;
            const long accountId = 666;
            const bool levyImported = true;
            const short periodMonth = 7;
            const string periodYear = "2018";


            return RunAsync(f => f.Handle(new RefreshEmployerLevyDataCompletedEvent
                {
                    AccountId = accountId,
                    LevyImported = levyImported,
                    PeriodMonth = periodMonth,
                    PeriodYear = periodYear,
                    Created = timestamp
                })
                , (f) =>
                {
                    f.VerifyRefreshEmployerLevyDataCompletedMessageIsPublished(accountId, levyImported, periodMonth, periodYear, timestamp);
                });
        }
    }

    public class RefreshEmployerLevyDataCompletedEventHandlerTestsFixture : FluentTestFixture
    {
        private readonly RefreshEmployerLevyDataCompletedEventHandler _handler;
        private readonly Mock<IMessagePublisher> _mockMessagePublisher;

        public RefreshEmployerLevyDataCompletedEventHandlerTestsFixture()
        {
            _mockMessagePublisher = new Mock<IMessagePublisher>();

            _handler = new RefreshEmployerLevyDataCompletedEventHandler(_mockMessagePublisher.Object);
        }

        public Task Handle(RefreshEmployerLevyDataCompletedEvent refreshEmployerLevyDataCompletedEvent)
        {
            return _handler.Handle(refreshEmployerLevyDataCompletedEvent, null);
        }

        public void VerifyRefreshEmployerLevyDataCompletedMessageIsPublished(long accountId, bool levyImported, short periodMonth, string periodYear, DateTime timestamp)
        {
            _mockMessagePublisher.Verify(e =>
                e.PublishAsync(It.Is<RefreshEmployerLevyDataCompletedMessage>(m =>
                    m.AccountId.Equals(accountId)
                    && m.LevyImported.Equals(levyImported)
                    && m.PeriodMonth.Equals(periodMonth)
                    && m.PeriodYear.Equals(periodYear)
                    && m.CreatedAt.Equals(timestamp))));
        }
    }
}
