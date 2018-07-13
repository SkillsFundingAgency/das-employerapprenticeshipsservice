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
    public class ApprovedTransferConnectionInvitationEventHandlerTests : FluentTest<ApprovedTransferConnectionInvitationEventHandlerTestFixture>
    {
        [Test]
        public Task Handle_WhenIGetAnApprovedTransferConnectionInvitationEvent_ThenAccountOwnersAreRetrieved()
        {
            return RunAsync(f => f.Handle(), f => f.EmployerAccountDbContext.Verify(u => u.Users, Times.Once));
        }

        [Test]
        public Task Handle_WhenIGetAnApprovedTransferConnectionInvitationEvent_ThenAccountOwnersRequiringNotificationAreNotified()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApi.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == ApprovedTransferConnectionInvitationEventHandlerTestFixture.ReceiverAccountName)),
                    Times.Exactly(2)));
        }

        [Test]
        public Task Handle_WhenIGetAnApprovedTransferConnectionInvitationEvent_ThenNotifyEmailCorrect()
        {
            return RunAsync(f => f.Handle(),
                f => f.NotificationsApi.Verify(
                    r => r.SendEmail(It.Is<Email>(e =>
                        e.RecipientsAddress == ApprovedTransferConnectionInvitationEventHandlerTestFixture.AccountOwner1.Email
                        && !string.IsNullOrWhiteSpace(e.Subject)
                        && e.ReplyToAddress == "noreply@sfa.gov.uk"
                        && e.TemplateId == "TransferConnectionRequestApproved"
                        && !string.IsNullOrWhiteSpace(e.Tokens["link_notification_page"])
                        && e.Tokens["account_name"] == ApprovedTransferConnectionInvitationEventHandlerTestFixture.ReceiverAccountName)),
                    Times.Once));
        }
    }

    public class ApprovedTransferConnectionInvitationEventHandlerTestFixture : FluentTestFixture
    {
        public Mock<EmployerAccountDbContext> EmployerAccountDbContext;

        private Account _senderAccount;

        public const string SenderAccountName = "SenderAccountName";

        public const long SenderAccountId = 111111;

        private Account _receiverAccount;

        public const string ReceiverAccountName = "ReceiverAccountName";

        public const long ReceiverAccountId = 22222;

        public ApprovedTransferConnectionInvitationEventHandler Handler { get; set; }

        public Mock<INotificationsApi> NotificationsApi { get; set; }

        public Mock<IMessageSubscriber<ApprovedTransferConnectionInvitationEvent>> MessageSubscriber { get; set; }

        public Mock<IMessageContextProvider> MessageContextProviderMock { get; set; }
        public IMessageContextProvider MessageContextProvider => MessageContextProviderMock.Object;

        public Mock<ILog> Logger { get; set; }

        public Mock<IMessageSubscriberFactory> MessageSubscriberFactory { get; set; }

        public readonly Mock<IMessage<ApprovedTransferConnectionInvitationEvent>> Message;

        public EmployerApprenticeshipsServiceConfiguration Configuration;

        public static User AccountOwner1 { get; set; }

        public static User AccountOwner2 { get; set; }

        public static User AccountOwner3NonNotify { get; set; }

        public static User AccountOwner4NonOwner { get; set; }

        public static User ReceiverAccountOwner { get; set; }

        public ApprovedTransferConnectionInvitationEvent Event { get; set; }

        private readonly CancellationTokenSource _cancellationTokenSource;

        private List<Account> _accounts;
        private List<User> _users;
        private List<UserAccountSetting> _userAccountSettings;
        private List<Membership> _memberships;
        private DbSetStub<Account> _accountsDbSet;
        private DbSetStub<UserAccountSetting> _userAccountSettingDbSet;
        private DbSetStub<User> _usersDbSet;
        private DbSetStub<Membership> _membershipsDbSet;


        public ApprovedTransferConnectionInvitationEventHandlerTestFixture()
        {
            Configuration = new EmployerApprenticeshipsServiceConfiguration();


            EmployerAccountDbContext = new Mock<EmployerAccountDbContext>();
            NotificationsApi = new Mock<INotificationsApi>();
            MessageSubscriberFactory = new Mock<IMessageSubscriberFactory>();
            MessageSubscriber = new Mock<IMessageSubscriber<ApprovedTransferConnectionInvitationEvent>>();
            Logger = new Mock<ILog>();
            MessageContextProviderMock = new Mock<IMessageContextProvider>();

            Message = new Mock<IMessage<ApprovedTransferConnectionInvitationEvent>>();

            SetMessage()
                .AddSenderAccount()
                .AddReceiverAccount()
                .SetAccountOwner1()
                .SetAccountOwner2()
                .SetAccountOwner3NonNotify()
                .SetAccountNonOwner()
                .SetReceiverAccountOwner();

            _cancellationTokenSource = new CancellationTokenSource();

            MessageSubscriber.Setup(s => s.ReceiveAsAsync()).ReturnsAsync(Message.Object)
                .Callback(_cancellationTokenSource.Cancel);

            MessageSubscriberFactory.Setup(s => s.GetSubscriber<ApprovedTransferConnectionInvitationEvent>())
                .Returns(MessageSubscriber.Object);

            Handler = new ApprovedTransferConnectionInvitationEventHandler(MessageSubscriberFactory.Object,
                Logger.Object, EmployerAccountDbContext.Object, NotificationsApi.Object, Configuration, MessageContextProvider);
        }

        private ApprovedTransferConnectionInvitationEventHandlerTestFixture SetMessage()
        {

            Event = new ApprovedTransferConnectionInvitationEvent
            {
                ReceiverAccountName = ReceiverAccountName,
                SenderAccountId = SenderAccountId
            };

            Message.Setup(m => m.Content).Returns(Event);

            return this;
        }


        private ApprovedTransferConnectionInvitationEventHandlerTestFixture AddSenderAccount()
        {
            _senderAccount = new Account
            {
                Id = SenderAccountId,
                Name = SenderAccountName
            };

            return AddAccount(_senderAccount);
        }

        private ApprovedTransferConnectionInvitationEventHandlerTestFixture AddReceiverAccount()
        {
            _receiverAccount = new Account
            {
                Id = ReceiverAccountId,
                Name = ReceiverAccountName
            };

            return AddAccount(_receiverAccount);
        }

        private ApprovedTransferConnectionInvitationEventHandlerTestFixture AddAccount(Account account)
        {
            if (_accounts == null)
                _accounts = new List<Account>();

            _accounts.Add(account);
            _accountsDbSet = new DbSetStub<Account>(_accounts);
            EmployerAccountDbContext.Setup(d => d.Accounts).Returns(_accountsDbSet);

            return this;
        }

        private ApprovedTransferConnectionInvitationEventHandlerTestFixture SetAccountOwner1()
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

        private ApprovedTransferConnectionInvitationEventHandlerTestFixture SetAccountOwner2()
        {
            AccountOwner2 = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 2,
                FirstName = "John2",
                LastName = "Doe2",
                Email = "JohnDoe2@zzzzzzzz.com",
            };

            return AddUserToMockDb(AccountOwner2, _senderAccount, Role.Owner, true);
        }


        private ApprovedTransferConnectionInvitationEventHandlerTestFixture SetAccountOwner3NonNotify()
        {
            AccountOwner3NonNotify = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 3,
                FirstName = "John3",
                LastName = "Doe3",
                Email = "JohnDoe3@zzzzzzzz.com"
            };

            return AddUserToMockDb(AccountOwner3NonNotify, _senderAccount, Role.Owner, false);
        }

        private ApprovedTransferConnectionInvitationEventHandlerTestFixture SetAccountNonOwner()
        {
            AccountOwner4NonOwner = new User
            {
                ExternalId = Guid.NewGuid(),
                Id = 4,
                FirstName = "John3",
                LastName = "Doe3",
                Email = "JohnDoe3@zzzzzzzz.com"
            };

            return AddUserToMockDb(AccountOwner4NonOwner, _senderAccount, Role.None, true);
        }

        private ApprovedTransferConnectionInvitationEventHandlerTestFixture SetReceiverAccountOwner()
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

        private ApprovedTransferConnectionInvitationEventHandlerTestFixture AddUserToMockDb(User user, Account account, Role role, bool receiveNotifications)
        {
            if(_users == null)
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
