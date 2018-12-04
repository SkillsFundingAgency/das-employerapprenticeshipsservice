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
    internal class RemoveAccountUserCommandHandlerTests : FluentTest<RemoveUserRolesCommandHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenUserHasNotBeenRemoved_ThenShouldMarkUserAsRemovedAndClearRoles()
        {
            return TestAsync(
                f => f.AddMatchingNonRemovedUserWithOwnerRole(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRoleRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.Removed == f.Removed &&
                        p.Roles.Any() == false
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenUserHasNotBeenRemoved_ThenShouldAddMessageIdToOutbox()
        {
            return TestAsync(
                f => f.AddMatchingNonRemovedUserWithOwnerRole(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRoleRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.OutboxData.Count(o => o.MessageId == f.MessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenUserHasAlreadyBeenRemoved_ThenShouldThrowException()
        {
            return TestExceptionAsync(
                f => f.AddMatchingAlreadyRemovedUser(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                (f, r) => r.ShouldThrow<InvalidOperationException>());
        }

        [Test]
        public Task Handle_WhenDeleteCommandArrivesOutOfOrderAfterALaterUpdateCommand_ThenSwallowMessageWithoutAction()
        {
            return TestAsync(
                f => f.AddMatchingRecentlyUpdatedUserWithViewerRole(),
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRoleRepository.Verify(x => x.Update(It.Is<AccountUser>(p =>
                        p.Removed == null &&
                        p.Roles.Any() &&
                        p.OutboxData.Count(o => o.MessageId == f.MessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }

        [Test]
        public Task Handle_WhenDeleteCommandArrivesAndNoMatchingUserRecordFound_ThrowException()
        {
            return TestExceptionAsync(
                f => f.Handler.Handle(f.Command, CancellationToken.None),
                (f, r) => r.ShouldThrow<Exception>());
        }
    }

    internal class RemoveUserRolesCommandHandlerTestsFixture
    {
        public string MessageId = "messageId";
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();
        public HashSet<UserRole> Roles = new HashSet<UserRole> { UserRole.Owner };
        public DateTime Removed = DateTime.Now.AddMinutes(-1);
        public List<AccountUser> Users;

        public Mock<IAccountUsersRepository> UserRoleRepository;

        public RemoveAccountUserCommand Command;
        public RemoveAccountUserCommandHandler Handler;

        public RemoveUserRolesCommandHandlerTestsFixture()
        {
            Users = new List<AccountUser>();

            UserRoleRepository = new Mock<IAccountUsersRepository>();
            UserRoleRepository.SetupInMemoryCollection(Users);

            Handler = new RemoveAccountUserCommandHandler(UserRoleRepository.Object);

            Command = new RemoveAccountUserCommand(AccountId, UserRef, MessageId, Removed);
        }

        public RemoveUserRolesCommandHandlerTestsFixture AddMatchingNonRemovedUserWithOwnerRole()
        {
            Users.Add(CreateBasicUser().Add(x=>x.Roles, UserRole.Owner));

            return this;
        }

        public RemoveUserRolesCommandHandlerTestsFixture AddMatchingAlreadyRemovedUser()
        {
            Users.Add(CreateBasicUser()
                .Set(x=>x.Removed, Removed.AddDays(-2)));

            return this;
        }

        public RemoveUserRolesCommandHandlerTestsFixture AddMatchingRecentlyUpdatedUserWithViewerRole()
        {
            Users.Add(CreateBasicUser()
                .Add(x => x.Roles, UserRole.Viewer)
                .Set(x => x.Updated, Removed.AddDays(1)));

            return this;
        }

        private AccountUser CreateBasicUser()
        {
            return ObjectActivator.CreateInstance<AccountUser>()
                .Set(x => x.AccountId, AccountId)
                .Set(x => x.Created, Removed.AddDays(-10))
                .Set(x => x.UserRef, UserRef);
        }
    }
}
