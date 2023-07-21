using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerFinance;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers.EmployerFinance
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

            return TestAsync(f => f.Handle(new RefreshEmployerLevyDataCompletedEvent
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

        [TestCase(0, ApprenticeshipEmployerType.NonLevy)]
        [TestCase(100, ApprenticeshipEmployerType.Levy)]
        public Task WhenMessageIsHandled_AccountLevyStatusCommandIsSent(decimal levyValue, ApprenticeshipEmployerType apprenticeshipEmployerType)
        {
            var timestamp = DateTime.UtcNow;
            const long accountId = 666;
            const bool levyImported = true;
            const short periodMonth = 7;
            const string periodYear = "2018";

            return TestAsync(f => f.Handle(new RefreshEmployerLevyDataCompletedEvent
            {
                AccountId = accountId,
                LevyImported = levyImported,
                PeriodMonth = periodMonth,
                PeriodYear = periodYear,
                LevyTransactionValue = levyValue,
                Created = timestamp
            })
                , (f) =>
                {
                    f.VerifyAccountLevyStatusCommandIsSent(accountId, apprenticeshipEmployerType);
                });
        }
    }

    public class RefreshEmployerLevyDataCompletedEventHandlerTestsFixture
    {
        private readonly RefreshEmployerLevyDataCompletedEventHandler _handler;
        private readonly Mock<ILegacyTopicMessagePublisher> _mockEventPublisher;
        private readonly Mock<IMediator> _mediator;

        public RefreshEmployerLevyDataCompletedEventHandlerTestsFixture()
        {
            _mockEventPublisher = new Mock<ILegacyTopicMessagePublisher>();
            _mediator = new Mock<IMediator>();

            _handler = new RefreshEmployerLevyDataCompletedEventHandler(_mockEventPublisher.Object, _mediator.Object);
        }

        public Task Handle(RefreshEmployerLevyDataCompletedEvent refreshEmployerLevyDataCompletedEvent)
        {
            return _handler.Handle(refreshEmployerLevyDataCompletedEvent, null);
        }

        public void VerifyRefreshEmployerLevyDataCompletedMessageIsPublished(long accountId, bool levyImported, short periodMonth, string periodYear, DateTime timestamp)
        {
            _mockEventPublisher.Verify(e =>
                e.PublishAsync(It.Is<RefreshEmployerLevyDataCompletedMessage>(m =>
                    m.AccountId.Equals(accountId)
                    && m.LevyImported.Equals(levyImported)
                    && m.PeriodMonth.Equals(periodMonth)
                    && m.PeriodYear.Equals(periodYear)
                    && m.CreatedAt.Equals(timestamp))));
        }

        public void VerifyAccountLevyStatusCommandIsSent(long accountId, ApprenticeshipEmployerType apprenticeshipEmployerType)
        {
            _mediator.Verify(e => e.Send(It.Is<AccountLevyStatusCommand>(m =>
                m.AccountId.Equals(accountId) &&
                m.ApprenticeshipEmployerType.Equals(apprenticeshipEmployerType)), CancellationToken.None),
                Times.Once);
        }
    }
}
