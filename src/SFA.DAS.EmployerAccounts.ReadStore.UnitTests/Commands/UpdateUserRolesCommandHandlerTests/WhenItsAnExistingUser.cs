using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Builders;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Commands.UpdateUserRolesCommandHandlerTests
{
    internal class WhenItsAnExistingUser : FluentTest<WhenItsAnExistingUserFixture>
    {
        [Test]
        public Task Handle_ThenShouldUpdateDocumentInRepository()
        {
            return RunAsync(f => f.AddMatchingUser(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<UserRoles>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.Roles.Equals(f.NewRoles) &&
                        p.Updated == f.Updated
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_ThenShouldAddMessageIdToOutbox()
        {
            return RunAsync(f => f.AddMatchingUser(), 
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<UserRoles>(p =>
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_ADuplicateMessageId_ThenShouldSimplyIgnoreTheUpdate()
        {
            return RunAsync(f => f.AddMatchingUserWithMessageAlreadyProcessed(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<UserRoles>(p =>
                        p.Roles.Contains(UserRole.Owner) == false && 
                        p.OutboxData.Count() == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_AnOldMessage_ThenShouldSimplySwallowTheMessageAndAddItToTheOutbox()
        {
            return RunAsync(f => f.AddMatchingUserWhichWasUpdatedLaterThanNewMessage(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<UserRoles>(p =>
                        p.Roles.Contains(UserRole.Owner) == false &&
                        p.OutboxData.Count() == 2 &&
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_AnOldMessageToADeletedUser_ThenShouldSimplySwallowTheMessageAndAddItToTheOutbox()
        {
            return RunAsync(f => f.AddMatchingUserWhichWasDeletedLaterThanNewMessage(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<UserRoles>(p =>
                        p.Roles.Contains(UserRole.Owner) == false &&
                        p.Deleted != null &&
                        p.OutboxData.Count() == 2 &&
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_ANewerMessageToADeletedRelationship_ThenShouldUndeleteAndAddItToTheOutbox()
        {
            return RunAsync(f => f.AddMatchingUserWhichWasDeletedEarlierThanNewMessage(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<UserRoles>(p =>
                        p.Roles.Contains(UserRole.Owner) &&
                        p.Deleted == null &&
                        p.OutboxData.Count() == 2 &&
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }


    }

    internal class WhenItsAnExistingUserFixture
    {
        public string FirstMessageId = "firstMessageId";
        public string UpdateMessageId = "updateMessageId";
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();
        public HashSet<UserRole> NewRoles = new HashSet<UserRole> { UserRole.Owner };
        public DateTime Updated = DateTime.Now.AddMinutes(-1);

        public Mock<IUsersRolesRepository> UserRolesRepository;
        public List<UserRoles> Users = new List<UserRoles>();

        public UpdateUserRolesCommand Command;
        public UpdateUserRolesCommandHandler Handler;

        public WhenItsAnExistingUserFixture()
        {
            UserRolesRepository = new Mock<IUsersRolesRepository>();
            UserRolesRepository.SetupInMemoryCollection(Users);

            Handler = new UpdateUserRolesCommandHandler(UserRolesRepository.Object);

            Command = new UpdateUserRolesCommand(AccountId, UserRef, NewRoles, UpdateMessageId, Updated);
        }

        public WhenItsAnExistingUserFixture AddMatchingUserWhichWasDeletedLaterThanNewMessage()
        {
            Users.Add(CreateBasicUser()
                .WithOutboxMessage(new OutboxMessage(FirstMessageId, Updated))
                .WithDeleted(Updated.AddHours(1))
                .Build());

            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUserWhichWasUpdatedLaterThanNewMessage()
        {
            Users.Add(CreateBasicUser()
                .WithOutboxMessage(new OutboxMessage(FirstMessageId, Updated))
                .WithUpdated(Updated.AddHours(1))
                .Build());

            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUserWhichWasDeletedEarlierThanNewMessage()
        {
            Users.Add(CreateBasicUser()
                .WithOutboxMessage(new OutboxMessage(FirstMessageId, Updated))
                .WithUpdated(Updated.AddHours(-1))
                .Build());

            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUserWithMessageAlreadyProcessed()
        {
            Users.Add(CreateBasicUser()
                .WithOutboxMessage(new OutboxMessage(UpdateMessageId, Updated))
                .Build());

            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUser()
        {
            Users.Add(CreateBasicUser().Build());
            return this;
        }

        private UserRolesBuilder CreateBasicUser()
        {
            return new UserRolesBuilder()
                .WithAccountId(AccountId)
                .WithUserRef(UserRef);
        }
    }
}
