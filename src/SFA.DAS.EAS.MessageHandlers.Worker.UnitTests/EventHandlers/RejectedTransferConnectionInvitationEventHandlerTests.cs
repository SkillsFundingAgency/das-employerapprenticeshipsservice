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
    public class RejectedTransferConnectionInvitationEventHandlerTests : FluentTest<RejectedTransferConnectionInvitationEventHandlerTestFixture>
    {
        [Test]
        public Task Handle_WhenIGetAnRejectedTransferConnectionInvitationEvent_ThenAccountOwnersAreRetrieved()
        {
            return RunAsync(f => f.Handle(), f => f.EmployerAccountDbContext.Verify(u => u.Users, Times.Once));
        }

        [Test]
        public Task Handle_WhenIGetAnRejectedTransferConnectionInvitationEvent_ThenAccountOwnersRequiringNotificationAreNotified()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApi.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == RejectedTransferConnectionInvitationEventHandlerTestFixture.ReceiverAccountName)),
                    Times.Exactly(2)));
        }

        [Test]
        public Task Handle_WhenIGetAnRejectedTransferConnectionInvitationEvent_ThenNotifyEmailCorrect()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApi.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        e.RecipientsAddress == RejectedTransferConnectionInvitationEventHandlerTestFixture.AccountOwner1.Email
                        && !string.IsNullOrWhiteSpace(e.Subject)
                        && e.ReplyToAddress == "noreply@sfa.gov.uk"
                        && e.TemplateId == "TransferConnectionRequestRejected"
                        && !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == RejectedTransferConnectionInvitationEventHandlerTestFixture.ReceiverAccountName)),
                    Times.Once));
        }
    }


    public class RejectedTransferConnectionInvitationEventHandlerTestFixture : FluentTestFixture
    {
        public Mock<EmployerAccountDbContext> EmployerAccountDbContext;

        public const string SenderAccountName = "TestAccountName";

        public const string ReceiverAccountName = "RecieverAccountName";

        public const long SenderAccountId = 111111;

        public List<UserAccountSetting> AccountOwners = new List<UserAccountSetting>();

        public RejectedTransferConnectionInvitationEventHandler Handler { get; set; }

        public Mock<INotificationsApi> NotificationsApi { get; set; }

        public Mock<IMessageSubscriber<RejectedTransferConnectionInvitationEvent>> MessageSubscriber { get; set; }

        public Mock<ILog> Logger { get; set; }

        public Mock<IMessageSubscriberFactory> MessageSubscriberFactory { get; set; }

        public Mock<IMessage<RejectedTransferConnectionInvitationEvent>> Message;

        public EmployerApprenticeshipsServiceConfiguration Configuration;

        public static User AccountOwner1 { get; set; }

        public static User AccountOwner2 { get; set; }

        public static User AccountOwner3NonNotify { get; set; }

        public static User AccountOwner4NonOwner { get; set; }

        public RejectedTransferConnectionInvitationEvent Event { get; set; }

        private readonly CancellationTokenSource _cancellationTokenSource;

        private List<Account> _accounts;
        private List<User> _users;
        private List<UserAccountSetting> _userAccountSettings;
        private List<Membership> _memberships;
        private Account _senderAccount;
        private DbSetStub<Account> _accountsDbSet;
        private DbSetStub<UserAccountSetting> _userAccountSettingDbSet;
        private DbSetStub<User> _usersDbSet;
        private DbSetStub<Membership> _membershipsDbSet;


        public RejectedTransferConnectionInvitationEventHandlerTestFixture()
        {
            Configuration = new EmployerApprenticeshipsServiceConfiguration();
            EmployerAccountDbContext = new Mock<EmployerAccountDbContext>();
            NotificationsApi = new Mock<INotificationsApi>();
            MessageSubscriberFactory = new Mock<IMessageSubscriberFactory>();
            MessageSubscriber = new Mock<IMessageSubscriber<RejectedTransferConnectionInvitationEvent>>();
            Logger = new Mock<ILog>();

            Message = new Mock<IMessage<RejectedTransferConnectionInvitationEvent>>();

            SetMessage()
                .AddAccount()
                .SetAccountOwner1()
                .SetAccountOwner2()
                .SetAccountOwner3NonNotify()
                .SetAccountNonOwner();

            _cancellationTokenSource = new CancellationTokenSource();

            MessageSubscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(Message.Object)
                .Callback(_cancellationTokenSource.Cancel);

            MessageSubscriberFactory.Setup(s => s.GetSubscriber<RejectedTransferConnectionInvitationEvent>())
                .Returns(MessageSubscriber.Object);

            Handler = new RejectedTransferConnectionInvitationEventHandler(MessageSubscriberFactory.Object,
                Logger.Object, new Lazy<EmployerAccountDbContext>(() => EmployerAccountDbContext.Object), NotificationsApi.Object, Configuration);
        }

        private RejectedTransferConnectionInvitationEventHandlerTestFixture SetMessage()
        {

            Event = new RejectedTransferConnectionInvitationEvent
            {
                ReceiverAccountName = ReceiverAccountName,
                SenderAccountId = SenderAccountId
            };

            Message.Setup(m => m.Content).Returns(Event);

            return this;
        }

        private RejectedTransferConnectionInvitationEventHandlerTestFixture SetAccountOwner1()
        {
            AccountOwner1 = new User
            {
                Ref = Guid.NewGuid(),
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoe@zzzzzzzz.com",
            };

            return AddUserToMockDb(AccountOwner1, Role.Owner, true);
        }

        private RejectedTransferConnectionInvitationEventHandlerTestFixture SetAccountOwner2()
        {
            AccountOwner2 = new User
            {
                Ref = Guid.NewGuid(),
                Id = 2,
                FirstName = "John2",
                LastName = "Doe2",
                Email = "JohnDoe2@zzzzzzzz.com",
            };

            return AddUserToMockDb(AccountOwner2, Role.Owner, true);
        }

        private RejectedTransferConnectionInvitationEventHandlerTestFixture AddAccount()
        {
            if (_accounts != null)
                return this;

            _senderAccount = new Account
            {
                Id = SenderAccountId,
                Name = SenderAccountName
            };

            _accounts = new List<Account>
            {
                _senderAccount
            };

            _accountsDbSet = new DbSetStub<Account>(_accounts);
            EmployerAccountDbContext.Setup(d => d.Accounts).Returns(_accountsDbSet);

            return this;
        }

        private RejectedTransferConnectionInvitationEventHandlerTestFixture SetAccountOwner3NonNotify()
        {
            AccountOwner3NonNotify = new User
            {
                Ref = Guid.NewGuid(),
                Id = 3,
                FirstName = "John3",
                LastName = "Doe3",
                Email = "JohnDoe3@zzzzzzzz.com"
            };

            return AddUserToMockDb(AccountOwner3NonNotify, Role.Owner, false);
        }

        private RejectedTransferConnectionInvitationEventHandlerTestFixture SetAccountNonOwner()
        {
            AccountOwner4NonOwner = new User
            {
                Ref = Guid.NewGuid(),
                Id = 4,
                FirstName = "John3",
                LastName = "Doe3",
                Email = "JohnDoe3@zzzzzzzz.com"
            };

            return AddUserToMockDb(AccountOwner4NonOwner, Role.None, true);
        }

        private RejectedTransferConnectionInvitationEventHandlerTestFixture AddUserToMockDb(User user, Role role, bool receiveNotifications)
        {
            if (_users == null)
                _users = new List<User>();

            _users.Add(user);

            if (_memberships == null)
                _memberships = new List<Membership>();

            var membership = new Membership
            {
                Account = _senderAccount,
                AccountId = _senderAccount.Id,
                User = user,
                UserId = user.Id,
                Role = role
            };
            _memberships.Add(membership);
            _membershipsDbSet = new DbSetStub<Membership>(_memberships);
            EmployerAccountDbContext.Setup(d => d.UserAccountSettings).Returns(_userAccountSettingDbSet);

            if (_userAccountSettings == null)
                _userAccountSettings = new List<UserAccountSetting>();

            var userAccountSetting = new TestUserAccountSetting(_senderAccount, user, receiveNotifications);
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
