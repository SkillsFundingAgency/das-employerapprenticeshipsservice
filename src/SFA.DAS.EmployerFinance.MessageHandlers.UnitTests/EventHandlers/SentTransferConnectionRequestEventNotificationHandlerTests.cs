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
    public class SentTransferConnectionRequestEventNoticiationHandlerTests : FluentTest<SentTransferConnectionRequestEventNotificationHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenSentTransferConnectionRequestEventIsHandled_ThenShouldNotifyAccountOwnersRequiringNotification()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApiClient.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == f.SenderAccount.Name)),
                    Times.Exactly(3)));
        }

        [Test]
        public Task Handle_WhenSentTransferConnectionRequestEventIsHandled_ThenShouldSentNotificationWithCorrectProperties()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApiClient.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        e.RecipientsAddress == f.ReceiverAccountOwner.Email
                        && !string.IsNullOrWhiteSpace(e.Subject)
                        && e.ReplyToAddress == "noreply@sfa.gov.uk"
                        && e.TemplateId == "TransferConnectionInvitationSent"
                        && !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == f.SenderAccount.Name)),
                    Times.Once));
        }
    }

    public class SentTransferConnectionRequestEventNotificationHandlerTestsFixture
        : TransferConnectionRequestEventNotificationHandlerTestsFixtureBase
    {
        public SentTransferConnectionRequestEventNotificationHandler Handler { get; set; }

        public SentTransferConnectionRequestEvent Event { get; set; }
     
        public SentTransferConnectionRequestEventNotificationHandlerTestsFixture()
        {
            AddSenderAccount();
            AddReceiverAccount();
            SetReceiverAccountOwner();
            SetSenderAccountOwner1();
            SetSenderAccountOwner2();
            SetMessage();

            Handler = new SentTransferConnectionRequestEventNotificationHandler(
                Configuration,
                OuterApiClient.Object,
                Logger.Object,
                NotificationsApiClient.Object);
        }
        
        public Task Handle()
        {
            return Handler.Handle(Event, null);
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestsFixture SetMessage()
        {
            Event = new SentTransferConnectionRequestEvent
            {
                ReceiverAccountId = ReceiverAccount.Id,
                SenderAccountId = SenderAccount.Id,
                SenderAccountName = SenderAccount.Name
            };

            return this;
        }
    }
}