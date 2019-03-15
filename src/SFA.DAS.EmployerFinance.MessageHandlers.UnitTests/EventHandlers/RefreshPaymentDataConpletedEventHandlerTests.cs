using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers.UnitTests
{
    [TestFixture]
    public class RefreshPaymentDataConpletedEventHandlerTests : FluentTest<RefreshPaymentDataConpletedEventHandlerTestsFixture>
    {
        [Test]
        public Task WhenMessageIsHandled_RefreshEmployerLevyDataCompletedMessageIsPublished()
        {
            var timestamp = DateTime.UtcNow;
            const long accountId = 666;
            const bool levyImported = true;
            const string periodEnd = "2018R03";


            return RunAsync(f => f.Handle(new RefreshPaymentDataCompletedEvent()
                {
                    AccountId = accountId,
                    PaymentsProcessed = levyImported,
                    PeriodEnd = periodEnd,
                    Created = timestamp
                })
                , (f) =>
                {
                    f.VerifyRefreshPaymentDataCompletedMessageIsPublished(accountId, levyImported, periodEnd, timestamp);
                });
        }
    }

    public class RefreshPaymentDataConpletedEventHandlerTestsFixture : FluentTestFixture
    {
        private readonly RefreshPaymentDataCompletedEventHandler _handler;
        private readonly Mock<IMessagePublisher> _mockMessagePublisher;

        public RefreshPaymentDataConpletedEventHandlerTestsFixture()
        {
            _mockMessagePublisher = new Mock<IMessagePublisher>();

            _handler = new RefreshPaymentDataCompletedEventHandler(_mockMessagePublisher.Object);
        }

        public Task Handle(RefreshPaymentDataCompletedEvent refreshEmployerLevyDataCompletedEvent)
        {
            return _handler.Handle(refreshEmployerLevyDataCompletedEvent, null);
        }

        public void VerifyRefreshPaymentDataCompletedMessageIsPublished(long accountId, bool paymentsProcessed, string payrollPeriod, DateTime timestamp)
        {
            _mockMessagePublisher.Verify(e =>
                e.PublishAsync(It.Is<RefreshPaymentDataCompletedMessage>(m =>
                    m.AccountId.Equals(accountId)
                    && m.PaymentsProcessed.Equals(paymentsProcessed)
                    && m.PayrollPeriod.Equals(payrollPeriod)
                    && m.CreatedAt.Equals(timestamp))));
        }
    }
}
