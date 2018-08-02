using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Testing;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    public class ApprovedTransferConnectionRequestEventNotificationHandlerTests : FluentTest<ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture>
    {
        [Test]
        public Task Handle_WhenApprovedTransferConnectionRequestEventIsHandled_ThenShouldNotifyAccountOwnersRequiringNotification()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApiClient.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == f.ReceiverAccount.Name)),
                    Times.Exactly(2)));
        }

        [Test]
        public Task Handle_WhenApprovedTransferConnectionRequestEventIsHandled_ThenShouldSentNotificationWithCorrectProperties()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApiClient.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        e.RecipientsAddress == f.SenderAccountOwner1.Email
                        && !string.IsNullOrWhiteSpace(e.Subject)
                        && e.ReplyToAddress == "noreply@sfa.gov.uk"
                        && e.TemplateId == "TransferConnectionRequestApproved"
                        && !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == f.ReceiverAccount.Name)),
                    Times.Once));
        }
    }

    public class ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture : FluentTestFixture
    {
        public ApprovedTransferConnectionRequestEventNotificationHandler Handler { get; set; }
        public EmployerAccountsConfiguration Configuration { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public Mock<ILog> Logger { get; set; }
        public Mock<INotificationsApi> NotificationsApiClient { get; set; }
        public ApprovedTransferConnectionRequestEvent Event { get; set; }
        public Account SenderAccount { get; set; }
        public User ReceiverAccountOwner { get; set; }
        public Account ReceiverAccount { get; set; }
        public User SenderAccountOwner1 { get; set; }
        public User SenderAccountOwner2 { get; set; }
        public User SenderAccountOwner3NonNotify { get; set; }
        public User SenderAccountOwner4NonOwner { get; set; }

        private readonly List<User> _users = new List<User>();

        public ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture()
        {
            Configuration = new EmployerAccountsConfiguration();
            Db = new Mock<EmployerAccountsDbContext>();
            Logger = new Mock<ILog>();
            NotificationsApiClient = new Mock<INotificationsApi>();

            Db.Setup(d => d.Users).Returns(new DbSetStub<User>(_users));

            AddSenderAccount()
                .SetSenderAccountOwner1()
                .SetSenderAccountOwner2()
                .SetSenderAccountOwner3NonNotify()
                .SetSenderAccountNonOwner()
                .AddReceiverAccount()
                .SetReceiverAccountOwner()
                .SetMessage();

            Handler = new ApprovedTransferConnectionRequestEventNotificationHandler(
                Configuration,
                Logger.Object,
                NotificationsApiClient.Object,
                new Lazy<EmployerAccountsDbContext>(() => Db.Object));
        }

        public Task Handle()
        {
            return Handler.Handle(Event, null);
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture SetMessage()
        {
            Event = new ApprovedTransferConnectionRequestEvent
            {
                ReceiverAccountId = ReceiverAccount.Id,
                ReceiverAccountName = ReceiverAccount.Name,
                SenderAccountId = SenderAccount.Id
            };

            return this;
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture AddSenderAccount()
        {
            SenderAccount = new Account
            {
                Id = 111111,
                Name = "SenderAccountName"
            };

            return this;
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture AddReceiverAccount()
        {
            ReceiverAccount = new Account
            {
                Id = 2222222,
                Name = "ReceiverAccountName"
            };

            return this;
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture SetReceiverAccountOwner()
        {
            ReceiverAccountOwner = new User
            {
                Ref = Guid.NewGuid(),
                Id = 1,
                FirstName = "Johnreceiver",
                LastName = "Doereceiver",
                Email = "JohnDoereceiver@zzzzzzzz.com",
            };

            return AddUserToDb(ReceiverAccountOwner, ReceiverAccount, Role.Owner, true);
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture SetSenderAccountOwner1()
        {
            SenderAccountOwner1 = new User
            {
                Ref = Guid.NewGuid(),
                Id = 99,
                FirstName = "Johnsender",
                LastName = "Doesender",
                Email = "JohnDoesender@zzzzzzzz.com"
            };

            return AddUserToDb(SenderAccountOwner1, SenderAccount, Role.Owner, true);
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture SetSenderAccountOwner2()
        {
            SenderAccountOwner2 = new User
            {
                Ref = Guid.NewGuid(),
                Id = 2,
                FirstName = "Johnsender2",
                LastName = "Doesender2",
                Email = "JohnDoesender2@zzzzzzzz.com",
            };

            return AddUserToDb(SenderAccountOwner2, SenderAccount, Role.Owner, true);
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture SetSenderAccountOwner3NonNotify()
        {
            SenderAccountOwner3NonNotify = new User
            {
                Ref = Guid.NewGuid(),
                Id = 3,
                FirstName = "Johnsender3",
                LastName = "Doesender3",
                Email = "JohnDoesender3@zzzzzzzz.com"
            };

            return AddUserToDb(SenderAccountOwner3NonNotify, SenderAccount, Role.Owner, false);
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture SetSenderAccountNonOwner()
        {
            SenderAccountOwner4NonOwner = new User
            {
                Ref = Guid.NewGuid(),
                Id = 4,
                FirstName = "Johnsender4",
                LastName = "Doesender4",
                Email = "JohnDoesender4@zzzzzzzz.com"
            };

            return AddUserToDb(SenderAccountOwner4NonOwner, SenderAccount, Role.None, true);
        }

        private ApprovedTransferConnectionRequestEventNotificationHandlerTestFixture AddUserToDb(User user, Account account, Role role, bool receiveNotifications)
        {
            var membership = new Membership
            {
                Account = account,
                User = user,
                Role = role
            };

            var userAccountSetting = new UserAccountSetting(account, user, receiveNotifications);

            user.UserAccountSettings.Add(userAccountSetting);
            user.Memberships.Add(membership);
            _users.Add(user);

            return this;
        }
    }
}