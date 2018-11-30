using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Commands;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Commands.UpdateUserRolesCommandHandlerTests
{
    internal class WhenItsANewUser : FluentTest<WhenItsANewUserFixture>
    {
        [Test]
        public Task Handle_ThenShouldAddNewDocumentToRepository()
        {
            return TestAsync(f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRoleRepository.Verify(x => x.Add(It.Is<UserRoles>(p =>
                        p.AccountId == f.AccountId &&
                        p.UserRef == f.UserRef &&
                        p.Roles.Equals(f.Roles) &&
                        p.Updated == f.Updated
                    ), null,
                    It.IsAny<CancellationToken>())));
        }
        [Test]
        public Task Handle_ThenShouldAddMessageIdToOutbox()
        {
            return TestAsync(f => f.Handler.Handle(f.Command, CancellationToken.None),
                f => f.UserRoleRepository.Verify(x => x.Add(It.Is<UserRoles>(p =>
                        p.OutboxData.Count(o => o.MessageId == f.MessageId) == 1
                    ), null,
                    It.IsAny<CancellationToken>())));
        }
    }

    internal class WhenItsANewUserFixture
    {
        public string MessageId = "messageId";
        public long AccountId = 333333;
        public Guid UserRef = Guid.NewGuid();
        public HashSet<UserRole> Roles = new HashSet<UserRole> { UserRole.Owner };
        public DateTime Updated = DateTime.Now.AddMinutes(-1);

        public Mock<IUsersRolesRepository> UserRoleRepository;

        public UpdateUserRolesCommand Command;
        public UpdateUserRolesCommandHandler Handler;

        public WhenItsANewUserFixture()
        {
            UserRoleRepository = new Mock<IUsersRolesRepository>();
            UserRoleRepository.SetupInMemoryCollection(new List<UserRoles>());

            Handler = new UpdateUserRolesCommandHandler(UserRoleRepository.Object);

            Command = new UpdateUserRolesCommand(AccountId, UserRef, Roles, MessageId, Updated);
        }
    }
}
