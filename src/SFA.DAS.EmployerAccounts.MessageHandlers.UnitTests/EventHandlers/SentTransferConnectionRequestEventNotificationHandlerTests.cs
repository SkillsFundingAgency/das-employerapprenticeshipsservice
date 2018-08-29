using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Testing;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.UnitTests.EventHandlers
{
    [TestFixture]
    public class SentTransferConnectionRequestEventNoticiationHandlerTests : FluentTest<SentTransferConnectionRequestEventNotificationHandlerTestFixture>
    {
        [Test]
        public Task Handle_WhenSentTransferConnectionRequestEventIsHandled_ThenShouldNotifyAccountOwnersRequiringNotification()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApiClient.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == f.SenderAccount.Name)),
                    Times.Exactly(2)));
        }

        [Test]
        public Task Handle_WhenSentTransferConnectionRequestEventIsHandled_ThenShouldSentNotificationWithCorrectProperties()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApiClient.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        e.RecipientsAddress == f.ReceiverAccountOwner1.Email
                        && !string.IsNullOrWhiteSpace(e.Subject)
                        && e.ReplyToAddress == "noreply@sfa.gov.uk"
                        && e.TemplateId == "TransferConnectionInvitationSent"
                        && !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == f.SenderAccount.Name)),
                    Times.Once));
        }
    }

    public class SentTransferConnectionRequestEventNotificationHandlerTestFixture : FluentTestFixture
    {
        public SentTransferConnectionRequestEventNotificationHandler Handler { get; set; }
        public EmployerAccountsConfiguration Configuration { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public Mock<ILog> Logger { get; set; }
        public Mock<INotificationsApi> NotificationsApiClient { get; set; }
        public SentTransferConnectionRequestEvent Event { get; set; }
        public Account SenderAccount { get; set; }
        public User SenderAccountOwner { get; set; }
        public Account ReceiverAccount { get; set; }
        public User ReceiverAccountOwner1 { get; set; }
        public User ReceiverAccountOwner2 { get; set; }
        public User ReceiverAccountOwner3NonNotify { get; set; }
        public User ReceiverAccountOwner4NonOwner { get; set; }

        private readonly List<User> _users = new List<User>();

        public SentTransferConnectionRequestEventNotificationHandlerTestFixture()
        {
            Configuration = new EmployerAccountsConfiguration();
            Db = new Mock<EmployerAccountsDbContext>();
            Logger = new Mock<ILog>();
            NotificationsApiClient = new Mock<INotificationsApi>();
            
            Db.Setup(d => d.Users).Returns(new DbSetStub<User>(_users));

            AddSenderAccount()
                .SetSenderAccountOwner()
                .AddReceiverAccount()
                .SetReceiverAccountOwner1()
                .SetReceiverAccountOwner2()
                .SetReceiverAccountOwner3NonNotify()
                .SetReceiverAccountNonOwner()
                .SetMessage();

            Handler = new SentTransferConnectionRequestEventNotificationHandler(
                Configuration,
                Logger.Object,
                NotificationsApiClient.Object,
                new Lazy<EmployerAccountsDbContext>(() => Db.Object));
        }
        
        public Task Handle()
        {
            return Handler.Handle(Event, null);
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture SetMessage()
        {
            Event = new SentTransferConnectionRequestEvent
            {
                ReceiverAccountId = ReceiverAccount.Id,
                SenderAccountId = SenderAccount.Id,
                SenderAccountName = SenderAccount.Name
            };

            return this;
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture AddSenderAccount()
        {
            SenderAccount = new Account
            {
                Id = 111111,
                Name = "SenderAccountName"
            };

            return this;
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture AddReceiverAccount()
        {
            ReceiverAccount = new Account
            {
                Id = 2222222,
                Name = "ReceiverAccountName"
            };

            return this;
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture SetSenderAccountOwner()
        {
            SenderAccountOwner = new User
            {
                Ref = Guid.NewGuid(),
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe@zzzzzzzz.com",
            };

            return AddUserToDb(SenderAccountOwner, SenderAccount, Role.Owner, true);
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture SetReceiverAccountOwner1()
        {
            ReceiverAccountOwner1 = new User
            {
                Ref = Guid.NewGuid(),
                Id = 99,
                FirstName = "Johnreceiver",
                LastName = "Doereceiver",
                Email = "JohnDoereceiver@zzzzzzzz.com",
            };

            return AddUserToDb(ReceiverAccountOwner1, ReceiverAccount, Role.Owner, true);
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture SetReceiverAccountOwner2()
        {
            ReceiverAccountOwner2 = new User
            {
                Ref = Guid.NewGuid(),
                Id = 2,
                FirstName = "Johnreceiver2",
                LastName = "Doereceiver2",
                Email = "JohnDoereceiver2@zzzzzzzz.com",
            };

            return AddUserToDb(ReceiverAccountOwner2, ReceiverAccount, Role.Owner, true);
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture SetReceiverAccountOwner3NonNotify()
        {
            ReceiverAccountOwner3NonNotify = new User
            {
                Ref = Guid.NewGuid(),
                Id = 3,
                FirstName = "Johnreceiver3",
                LastName = "Doereceiver3",
                Email = "JohnDoereceiver3@zzzzzzzz.com"
            };

            return AddUserToDb(ReceiverAccountOwner3NonNotify, ReceiverAccount, Role.Owner, false);
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture SetReceiverAccountNonOwner()
        {
            ReceiverAccountOwner4NonOwner = new User
            {
                Ref = Guid.NewGuid(),
                Id = 4,
                FirstName = "Johnreceiver4",
                LastName = "Doereceiver4",
                Email = "JohnDoereceiver4@zzzzzzzz.com"
            };

            return AddUserToDb(ReceiverAccountOwner4NonOwner, ReceiverAccount, Role.None, true);
        }

        private SentTransferConnectionRequestEventNotificationHandlerTestFixture AddUserToDb(User user, Account account, Role role, bool receiveNotifications)
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