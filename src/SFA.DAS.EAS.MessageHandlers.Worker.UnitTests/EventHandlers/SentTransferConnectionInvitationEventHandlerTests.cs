using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.MessageHandlers.Worker.EventHandlers;
using SFA.DAS.EAS.MessageHandlers.Worker.UnitTests.TestModels;
using SFA.DAS.EAS.TestCommon;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EAS.MessageHandlers.Worker.UnitTests.EventHandlers
{
    [TestFixture]
    public class SentTransferConnectionInvitationEventHandlerTests : FluentTest<SentTransferConnectionInvitationEventHandlerTestFixture>
    {
        [Test]
        public Task Handle_WhenIGetAnSentTransferConnectionInvitationEvent_ThenAccountOwnersAreRetrieved()
        {
            return RunAsync(f => f.Handle(), f => f.EmployerAccountDbContext.Verify(u => u.Users, Times.Once));
        }

        [Test]
        public Task Handle_WhenIGetAnSentTransferConnectionInvitationEvent_ThenAccountOwnersRequiringNotificationAreNotified()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApi.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == SentTransferConnectionInvitationEventHandlerTestFixture.SenderAccountName
                        )),
                    Times.Exactly(2)));
        }

        [Test]
        public Task Handle_WhenIGetAnSentTransferConnectionInvitationEvent_ThenNotifyEmailCorrect()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApi.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        e.RecipientsAddress == SentTransferConnectionInvitationEventHandlerTestFixture.ReceiverAccountOwner.Email
                        && !string.IsNullOrWhiteSpace(e.Subject)
                        && e.ReplyToAddress == "noreply@sfa.gov.uk"
                        && e.TemplateId == "TransferConnectionInvitationSent"
                        && !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == SentTransferConnectionInvitationEventHandlerTestFixture.SenderAccountName)),
                    Times.Once));
        }
    }

    public class SentTransferConnectionInvitationEventHandlerTestFixture : FluentTestFixture
    {
        public Mock<EmployerAccountDbContext> EmployerAccountDbContext;

        private Account _senderAccount;

        public const string SenderAccountName = "SenderAccountName";

        public const long SenderAccountId = 111111;

        private Account _receiverAccount;

        public const string ReceiverAccountName = "ReceiverAccountName";

        public const long ReceiverAccountId = 2222222;

        public SentTransferConnectionInvitationEventHandler Handler { get; set; }

        public Mock<INotificationsApi> NotificationsApi { get; set; }

        public Mock<IMessageSubscriber<SentTransferConnectionInvitationEvent>> MessageSubscriber { get; set; }

        public Mock<IMessageContextProvider> MessageContextProvider { get; set; }

        public Mock<ILog> Logger { get; set; }

        public Mock<IMessageSubscriberFactory> MessageSubscriberFactory { get; set; }

        public readonly Mock<IMessage<SentTransferConnectionInvitationEvent>> Message;

        public EmployerApprenticeshipsServiceConfiguration Configuration;

        public static User AccountOwner1 { get; set; }

        public static User ReceiverAccountOwner { get; set; }

        public static User ReceiverAccountOwner2 { get; set; }

        public static User ReceiverAccountOwner3NonNotify { get; set; }

        public static User ReceiverAccountOwner4NonOwner { get; set; }

        public SentTransferConnectionInvitationEvent Event { get; set; }

        private readonly CancellationTokenSource _cancellationTokenSource;

        private List<Account> _accounts;
        private List<User> _users;
        private List<UserAccountSetting> _userAccountSettings;
        private List<Membership> _memberships;
        private DbSetStub<Account> _accountsDbSet;
        private DbSetStub<UserAccountSetting> _userAccountSettingDbSet;
        private DbSetStub<User> _usersDbSet;
        private DbSetStub<Membership> _membershipsDbSet;


        public SentTransferConnectionInvitationEventHandlerTestFixture()
        {
            Configuration = new EmployerApprenticeshipsServiceConfiguration();
            EmployerAccountDbContext = new Mock<EmployerAccountDbContext>();
            NotificationsApi = new Mock<INotificationsApi>();
            MessageSubscriberFactory = new Mock<IMessageSubscriberFactory>();
            MessageSubscriber = new Mock<IMessageSubscriber<SentTransferConnectionInvitationEvent>>();
            Logger = new Mock<ILog>();
            MessageContextProvider = new Mock<IMessageContextProvider>();

            Message = new Mock<IMessage<SentTransferConnectionInvitationEvent>>();

            SetMessage()
                .AddSenderAccount()
                .AddReceiverAccount()
                .SetSenderAccountOwner()
                .SetReceiverAccountOwner1()
                .SetReceiverAccountOwner2()
                .SetReceiverAccountOwner3NonNotify()
                .SetReceiverAccountNonOwner();


            _cancellationTokenSource = new CancellationTokenSource();

            MessageSubscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(Message.Object)
                .Callback(_cancellationTokenSource.Cancel);

            MessageSubscriberFactory.Setup(s => s.GetSubscriber<SentTransferConnectionInvitationEvent>())
                .Returns(MessageSubscriber.Object);

            Handler = new SentTransferConnectionInvitationEventHandler(MessageSubscriberFactory.Object,
                Logger.Object, EmployerAccountDbContext.Object, NotificationsApi.Object, Configuration, MessageContextProvider.Object);
        }

        private SentTransferConnectionInvitationEventHandlerTestFixture SetMessage()
        {

            Event = new SentTransferConnectionInvitationEvent
            {
                ReceiverAccountId = ReceiverAccountId,
                SenderAccountId = SenderAccountId,
                SenderAccountName = SenderAccountName
            };

            Message.Setup(m => m.Content).Returns(Event);

            return this;
        }


        private SentTransferConnectionInvitationEventHandlerTestFixture AddSenderAccount()
        {
            _senderAccount = new Account
            {
                Id = SenderAccountId,
                Name = SenderAccountName
            };

            return AddAccount(_senderAccount);
        }

        private SentTransferConnectionInvitationEventHandlerTestFixture AddReceiverAccount()
        {
            _receiverAccount = new Account
            {
                Id = ReceiverAccountId,
                Name = ReceiverAccountName
            };

            return AddAccount(_receiverAccount);
        }

        private SentTransferConnectionInvitationEventHandlerTestFixture AddAccount(Account account)
        {
            if (_accounts == null)
                _accounts = new List<Account>();

            _accounts.Add(account);
            _accountsDbSet = new DbSetStub<Account>(_accounts);
            EmployerAccountDbContext.Setup(d => d.Accounts).Returns(_accountsDbSet);

            return this;
        }

        private SentTransferConnectionInvitationEventHandlerTestFixture SetSenderAccountOwner()
        {
            AccountOwner1 = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe@zzzzzzzz.com",
            };

            return AddUserToMockDb(AccountOwner1, _senderAccount, Role.Owner, true);
        }

        private SentTransferConnectionInvitationEventHandlerTestFixture SetReceiverAccountOwner1()
        {
            ReceiverAccountOwner = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 99,
                FirstName = "Johnreceiver",
                LastName = "Doereceiver",
                Email = "JohnDoereceiver@zzzzzzzz.com",
            };

            return AddUserToMockDb(ReceiverAccountOwner, _receiverAccount, Role.Owner, true);
        }

        private SentTransferConnectionInvitationEventHandlerTestFixture SetReceiverAccountOwner2()
        {
            ReceiverAccountOwner2 = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 2,
                FirstName = "Johnreceiver2",
                LastName = "Doereceiver2",
                Email = "JohnDoereceiver2@zzzzzzzz.com",
            };

            return AddUserToMockDb(ReceiverAccountOwner2, _receiverAccount, Role.Owner, true);
        }

        private SentTransferConnectionInvitationEventHandlerTestFixture SetReceiverAccountOwner3NonNotify()
        {
            ReceiverAccountOwner3NonNotify = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 3,
                FirstName = "Johnreceiver3",
                LastName = "Doereceiver3",
                Email = "JohnDoereceiver3@zzzzzzzz.com"
            };

            return AddUserToMockDb(ReceiverAccountOwner3NonNotify, _receiverAccount, Role.Owner, false);
        }
        private SentTransferConnectionInvitationEventHandlerTestFixture SetReceiverAccountNonOwner()
        {
            ReceiverAccountOwner4NonOwner = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 4,
                FirstName = "Johnreceiver4",
                LastName = "Doereceiver4",
                Email = "JohnDoereceiver4@zzzzzzzz.com"
            };

            return AddUserToMockDb(ReceiverAccountOwner4NonOwner, _receiverAccount, Role.None, true);
        }

        private SentTransferConnectionInvitationEventHandlerTestFixture AddUserToMockDb(User user, Account account, Role role, bool receiveNotifications)
        {
            if (_users == null)
                _users = new List<User>();

            _users.Add(user);

            if (_memberships == null)
                _memberships = new List<Membership>();

            var membership = new Membership
            {
                Account = account,
                AccountId = account.Id,
                User = user,
                UserId = user.Id,
                Role = role
            };
            _memberships.Add(membership);
            _membershipsDbSet = new DbSetStub<Membership>(_memberships);
            EmployerAccountDbContext.Setup(d => d.UserAccountSettings).Returns(_userAccountSettingDbSet);

            if (_userAccountSettings == null)
                _userAccountSettings = new List<UserAccountSetting>();

            var userAccountSetting = new TestUserAccountSetting(account, user, receiveNotifications);
            _userAccountSettings.Add(userAccountSetting);
            _userAccountSettingDbSet = new DbSetStub<UserAccountSetting>(_userAccountSettings);
            EmployerAccountDbContext.Setup(d => d.Memberships).Returns(_membershipsDbSet);

            user.UserAccountSettings.Add(userAccountSetting);
            user.Memberships.Add(membership);
            _usersDbSet = new DbSetStub<User>(_users);
            EmployerAccountDbContext.Setup(d => d.Users).Returns(_usersDbSet);

            return this;
        }

        public async Task Handle()
        {
            await Handler.RunAsync(_cancellationTokenSource);
        }
    }
}
