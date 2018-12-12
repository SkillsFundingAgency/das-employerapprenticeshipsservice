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
    internal class CreateAccountUserCommandHandlerTests : FluentTest<CreateAccountUserCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenItsANewUser_ThenShouldAddNewDocumentToRepositoryWithANonEmptyId()
        {
            return TestAsync(f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Add(It.Is<AccountUser>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.Roles.Equals(f.NewRoles) &&
                        p.Created == f.Created && 
                        p.Id != Guid.Empty
                    ), null,
                    It.IsAny<CancellationToken>())));
        }
        [Test]
        public Task Handle_WhenItsANewUser_ThenShouldAddMessageIdToOutbox()
        {
            return TestAsync(f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Add(It.Is<AccountUser>(p =>
                        p.OutboxData.Count(o => o.MessageId == f.MessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenUserIsActive_ThenShouldThrowException()
        {
            return TestExceptionAsync(f => f.AddMatchingUser(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                (f, r) => r.ShouldThrow<InvalidOperationException>());
        }

        [Test]
        public Task Handle_WhenUserIsMarkedAsRemoved_ThenShouldAddRolesUpdateDateTimeFieldsAndAddMessageIdToOutbox()
        {
            return TestAsync(
                f => f.AddMatchingRemovedUser(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.Created == f.Created &&
                        p.Updated == null &&
                        p.Removed == null &&
                        p.OutboxData.Count(o => o.MessageId == f.MessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenUserIsActiveAndCommandIsADuplicateMessageId_ThenShouldSimplyIgnore()
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
        public Task Handle_WhenUserIsActiveAndThisIsAnOlderCreateCommand_ThenShouldSwallowMessageAndAddItToOutbox()
        {
            return TestAsync(f => f.AddMatchingViewUserWhichWasCreatedLaterThanNewMessage(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRolesRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.Roles.Contains(UserRole.Owner) == false &&
                        p.Roles.Contains(UserRole.Viewer) &&
                        p.OutboxData.Count(o=>o.MessageId == f.Command.MessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenUserIsActiveAndThisIsALaterCreateCommand_ThenShouldThrowException()
        {
            return TestExceptionAsync(f => f.AddMatchingViewUserWhichWasCreatedEarlierThanNewMessage(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                (f, r) => r.ShouldThrow<InvalidOperationException>());
        }
    }

    internal class CreateAccountUserCommandHandlerTestsFixture
    {
        public string FirstMessageId = "firstMessageId";
        public string MessageId = "reinvokeMessageId";
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();
        public long UserId = 76682;
        public HashSet<UserRole> NewRoles = new HashSet<UserRole> { UserRole.Owner };
        public DateTime Created = DateTime.Now.AddMinutes(-1);

        public Mock<IAccountUsersRepository> UserRolesRepository;
        public List<AccountUser> Users = new List<AccountUser>();

        public CreateAccountUserCommand Command;
        public CreateAccountUserCommandHandler Handler;

        public CreateAccountUserCommandHandlerTestsFixture()
        {
            UserRolesRepository = new Mock<IAccountUsersRepository>();
            UserRolesRepository.SetupInMemoryCollection(Users);

            Handler = new CreateAccountUserCommandHandler(UserRolesRepository.Object);

            Command = new CreateAccountUserCommand(AccountId, UserRef, NewRoles, MessageId, Created);
        }

        public CreateAccountUserCommandHandlerTestsFixture AddMatchingViewUserWhichWasCreatedLaterThanNewMessage()
        {
            Users.Add(CreateBasicUser()
                .Add(x => x.OutboxData, new OutboxMessage(FirstMessageId, Created))
                .Set(x => x.Created, Created.AddHours(1))
                .Add(x => x.Roles, UserRole.Viewer));
            return this;
        }

        public CreateAccountUserCommandHandlerTestsFixture AddMatchingViewUserWhichWasCreatedEarlierThanNewMessage()
        {
            Users.Add(CreateBasicUser()
                .Add(x => x.OutboxData, new OutboxMessage(FirstMessageId, Created))
                .Set(x => x.Created, Created.AddHours(-1))
                .Add(x => x.Roles, UserRole.Viewer));
            return this;
        }

        public CreateAccountUserCommandHandlerTestsFixture AddMatchingUserWithMessageAlreadyProcessed()
        {
            Users.Add(CreateBasicUser()
                .Add(x=>x.OutboxData, new OutboxMessage(MessageId, Created)));

            return this;
        }

        public CreateAccountUserCommandHandlerTestsFixture AddMatchingRemovedUser()
        {
            Users.Add(CreateBasicUser().Set(x=>x.Updated, Created.AddSeconds(-10)).Set(x=>x.Removed, Created.AddSeconds(-5)));
            return this;
        }

        public CreateAccountUserCommandHandlerTestsFixture AddMatchingUser()
        {
            Users.Add(CreateBasicUser());
            return this;
        }

        private AccountUser CreateBasicUser()
        {
            return ObjectActivator.CreateInstance<AccountUser>()
                .Set(x=>x.AccountId, AccountId)
                .Set(x=>x.UserRef, UserRef);
        }
    }
}