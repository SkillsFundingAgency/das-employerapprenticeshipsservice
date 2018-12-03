using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Commands
{
    internal class UpdateAccountUserCommandHandlerTests : FluentTest<WhenItsAnExistingUserFixture>
    {
        [Test]
        public Task Handle_WhenItsANewUser_ThenShouldAddNewDocumentToRepositoryWithANonEmptyId()
        {
            return TestAsync(f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Add(It.Is<AccountUser>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.UserId == f.UserId &&
                        p.Roles.Equals(f.NewRoles) &&
                        p.Created == f.Updated && 
                        p.Id != Guid.Empty
                    ), null,
                    It.IsAny<CancellationToken>())));
        }
        [Test]
        public Task Handle_WhenItsANewUser_ThenShouldAddMessageIdToOutbox()
        {
            return TestAsync(f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Add(It.Is<AccountUser>(p =>
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenItsAnExistingUser_ThenShouldUpdateDocumentInRepository()
        {
            return TestAsync(f => f.AddMatchingUser(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.UserId == f.UserId &&
                        p.Roles.Equals(f.NewRoles) &&
                        p.Updated == f.Updated
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenItsAnExistingUser_ThenShouldAddMessageIdToOutbox()
        {
            return TestAsync(f => f.AddMatchingUser(), 
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenItsAnExistingUserAndCommandIsADuplicateMessageId_ThenShouldSimplyIgnoreTheUpdate()
        {
            return TestAsync(f => f.AddMatchingUserWithMessageAlreadyProcessed(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.Roles.Contains(UserRole.Owner) == false && 
                        p.OutboxData.Count() == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenAnOutOfDateUpdateMessageIsProcessedAfterAMoreRecentUpdate_ThenShouldSimplySwallowTheMessageAndAddItToTheOutbox()
        {
            return TestAsync(f => f.AddMatchingUserWhichWasUpdatedLaterThanNewMessage(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.Roles.Contains(UserRole.Owner) == false &&
                        p.OutboxData.Count() == 2 &&
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenAnOutOfDateUpdateMessageIsProcessedAfterUserHasBeenRemoved_ThenShouldSimplySwallowTheMessageAndAddItToTheOutbox()
        {
            return TestAsync(f => f.AddMatchingUserWhichWasRemovedLaterThanNewMessage(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.Roles.Contains(UserRole.Owner) == false &&
                        p.Removed != null &&
                        p.OutboxData.Count() == 2 &&
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_ANewerMessageToARemovedUser_ThenShouldMakeUserActiveAgainAndAddMessageToTheOutbox()
        {
            return TestAsync(f => f.AddMatchingUserWhichWasRemovedEarlierThanNewMessage(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.Roles.Contains(UserRole.Owner) &&
                        p.Removed == null &&
                        p.OutboxData.Count() == 2 &&
                        p.OutboxData.Count(o => o.MessageId == f.UpdateMessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenUserIdDoesNotMatchTheSelectedUserDocument_ThenShouldThrowException()
        {
            return TestExceptionAsync(
                f => f.AddMatchingUserExceptForUserId(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                (f, r) => r.ShouldThrow<InvalidOperationException>());
        }
    }

    internal class WhenItsAnExistingUserFixture
    {
        public string FirstMessageId = "firstMessageId";
        public string UpdateMessageId = "updateMessageId";
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();
        public long UserId = 76682;
        public HashSet<UserRole> NewRoles = new HashSet<UserRole> { UserRole.Owner };
        public DateTime Updated = DateTime.Now.AddMinutes(-1);

        public Mock<IAccountUsersRepository> UserRolesRepository;
        public List<AccountUser> Users = new List<AccountUser>();

        public UpdateAccountUserCommand Command;
        public UpdateAccountUserCommandHandler Handler;

        public WhenItsAnExistingUserFixture()
        {
            UserRolesRepository = new Mock<IAccountUsersRepository>();
            UserRolesRepository.SetupInMemoryCollection(Users);

            Handler = new UpdateAccountUserCommandHandler(UserRolesRepository.Object);

            Command = new UpdateAccountUserCommand(AccountId, UserRef, UserId, NewRoles, UpdateMessageId, Updated);
        }

        public WhenItsAnExistingUserFixture AddMatchingUserWhichWasRemovedLaterThanNewMessage()
        {
            Users.Add(CreateBasicUser()
                .Add(x => x.OutboxData, new OutboxMessage(FirstMessageId, Updated))
                .Set(x=>x.Removed, Updated.AddHours(1)));

            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUserWhichWasUpdatedLaterThanNewMessage()
        {
            Users.Add(CreateBasicUser()
                .Add(x => x.OutboxData, new OutboxMessage(FirstMessageId, Updated))
                .Set(x=>x.Updated, Updated.AddHours(1)));

            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUserWhichWasRemovedEarlierThanNewMessage()
        {
            Users.Add(CreateBasicUser()
                .Add(x => x.OutboxData, new OutboxMessage(FirstMessageId, Updated))
                .Set(x => x.Updated, Updated.AddHours(-1)));

            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUserWithMessageAlreadyProcessed()
        {
            Users.Add(CreateBasicUser()
                .Add(x=>x.OutboxData, new OutboxMessage(UpdateMessageId, Updated)));

            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUserExceptForUserId()
        {
            Users.Add(CreateBasicUser().Set(x => x.UserId, UserId + 1));
            return this;
        }

        public WhenItsAnExistingUserFixture AddMatchingUser()
        {
            Users.Add(CreateBasicUser());
            return this;
        }

        private AccountUser CreateBasicUser()
        {
            return ObjectActivator.CreateInstance<AccountUser>()
                .Set(x=>x.AccountId, AccountId)
                .Set(x=>x.UserId, UserId)
                .Set(x=>x.UserRef, UserRef);
        }
    }
}