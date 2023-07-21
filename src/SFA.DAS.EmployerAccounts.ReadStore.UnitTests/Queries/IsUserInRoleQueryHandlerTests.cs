using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Queries
{
    [TestFixture]
    [Parallelizable]
    public class IsUserInRoleQueryHandlerTests : FluentTest<IsUserInRoleQueryHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenSingleMatchingUserFound_ShouldReturnTrue()
        {
            return TestAsync(f => f.AddSingleMatchingUser(), f => f.Handle(), (f, r) => r.Should().BeTrue());
        }

        [Test]
        public Task Handle_WhenNoMatchingUserFound_ShouldReturnFalse()
        {
            return TestAsync(f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenMultipleMatchingUsersFound_ShouldReturnTrue()
        {
            return TestAsync(f => f.AddMultipleMatchingUsers(), f => f.Handle(), (f, r) => r.Should().BeTrue());
        }

        [Test]
        public Task Handle_WhenNonMatchingBecauseOfUserRef_ShouldReturnFalse()
        {
            return TestAsync(f => f.AddNonMatchingOnUserRef(), f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenNotMatchingBecauseOfAccountId_ShouldReturnFalse()
        {
            return TestAsync(f => f.AddNonMatchingOnAccountId(), f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenNotMatchingBecauseOfRole_ShouldReturnFalse()
        {
            return TestAsync(f => f.AddNonMatchingOnRoleEnum(), f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenNotMatchingBecauseThereIsNoRole_ShouldReturnFalse()
        {
            return TestAsync(f => f.AddNonMatchingUsers(), f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenMultipleRolesArePassedAndOnlyOneMatches_ShouldReturnTrue()
        {
            return TestAsync(f => f.SetMultipleRolesInQuery().AddSingleMatchingUser(), f => f.Handle(), (f, r) => r.Should().BeTrue());
        }
    }

    public class IsUserInRoleQueryHandlerTestsFixture
    {
        internal IsUserInRoleQuery Query { get; set; }
        public CancellationToken CancellationToken { get; set; }
        internal IRequestHandler<IsUserInRoleQuery,bool> Handler { get; set; }
        internal Mock<IAccountUsersRepository> UserRolesRepository { get; set; }
        internal List<AccountUser> AccountUsers { get; set; }

        public IsUserInRoleQueryHandlerTestsFixture()
        {
            Query = new IsUserInRoleQuery(Guid.NewGuid(), 112, new HashSet<UserRole>{ UserRole.Owner });
            CancellationToken = CancellationToken.None;
            UserRolesRepository = new Mock<IAccountUsersRepository>();
            AccountUsers = new List<AccountUser>();
            UserRolesRepository.SetupInMemoryCollection(AccountUsers);

            Handler = new IsUserInRoleQueryHandler(UserRolesRepository.Object);
        }

        internal Task<bool> Handle()
        {
            return Handler.Handle(Query, CancellationToken);
        }

        public IsUserInRoleQueryHandlerTestsFixture SetMultipleRolesInQuery()
        {
            Query.UserRoles.ToList().Add(UserRole.Transactor);

            return this;
        }

        public IsUserInRoleQueryHandlerTestsFixture AddSingleMatchingUser()
        {
            AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole());

            return this;
        }

        public IsUserInRoleQueryHandlerTestsFixture AddMultipleMatchingUsers()
        {
            AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole());
            AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole());

            return this;
        }

        public IsUserInRoleQueryHandlerTestsFixture AddNonMatchingOnUserRef()
        {
            AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole().Set(x => x.UserRef, Guid.NewGuid()));

            return this;
        }

        public IsUserInRoleQueryHandlerTestsFixture AddNonMatchingOnAccountId()
        {
            AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole().Set(x=>x.AccountId, 214));

            return this;
        }

        public IsUserInRoleQueryHandlerTestsFixture AddNonMatchingOnRoleEnum()
        {
            AccountUsers.Add(CreateBasicMatchingAccountUserObject().Set(x=>x.Role, UserRole.Transactor)); //not matching on role

            return this;
        }

        public IsUserInRoleQueryHandlerTestsFixture AddNonMatchingUsers()
        {
            AccountUsers.Add(CreateBasicMatchingAccountUserObject()); //not matching no roles

            return this;
        }

        private AccountUser CreateBasicMatchingAccountUserWithOwnerRole()
        {
            return CreateBasicMatchingAccountUserObject().Set(x => x.Role, UserRole.Owner);
        }

        private AccountUser CreateBasicMatchingAccountUserObject()
        {
            return ObjectActivator.CreateInstance<AccountUser>()
                .Set(x => x.UserRef, Query.UserRef)
                .Set(x => x.AccountId, Query.AccountId);
        }
    }
}
