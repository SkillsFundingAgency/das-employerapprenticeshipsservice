using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    public class RejectedTransferConnectionRequestEventNotificationHandlerTests : FluentTest<RejectedTransferConnectionRequestEventNotificationHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenRejectedTransferConnectionRequestEventIsHandled_ThenShouldNotifyAccountOwnersRequiringNotification()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApiClient.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == f.ReceiverAccount.Name)),
                    Times.Exactly(3)));
        }

        [Test]
        public Task Handle_WhenRejectedTransferConnectionRequestEventIsHandled_ThenShouldSentNotificationWithCorrectProperties()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApiClient.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        e.RecipientsAddress == f.SenderAccountOwner1.Email
                        && !string.IsNullOrWhiteSpace(e.Subject)
                        && e.ReplyToAddress == "noreply@sfa.gov.uk"
                        && e.TemplateId == "TransferConnectionRequestRejected"
                        && !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == f.ReceiverAccount.Name)),
                    Times.Once));
        }
    }

    public class RejectedTransferConnectionRequestEventNotificationHandlerTestsFixture
        : TransferConnectionRequestEventNotificationHandlerTestsFixtureBase
    {
        public RejectedTransferConnectionRequestEventNotificationHandler Handler { get; set; }

        public RejectedTransferConnectionRequestEvent Event { get; set; }
        
        public RejectedTransferConnectionRequestEventNotificationHandlerTestsFixture()
        {
            AddSenderAccount();
            AddReceiverAccount();
            SetReceiverAccountOwner();
            SetSenderAccountOwner1();
            SetSenderAccountOwner2();
            SetMessage();

            Handler = new RejectedTransferConnectionRequestEventNotificationHandler(
                Configuration,
                OuterApiClient.Object,
                Logger.Object,
                NotificationsApiClient.Object);
        }

        public Task Handle()
        {
            return Handler.Handle(Event, null);
        }

        private RejectedTransferConnectionRequestEventNotificationHandlerTestsFixture SetMessage()
        {
            Event = new RejectedTransferConnectionRequestEvent
            {
                ReceiverAccountId = ReceiverAccount.Id,
                ReceiverAccountName = ReceiverAccount.Name,
                SenderAccountId = SenderAccount.Id
            };

            return this;
        }
    }
}