using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Queries
{
    [TestFixture]
    [Parallelizable]
    public class HasRoleQueryHandlerTests : FluentTest<HasRoleQueryHandlerTestsFixture>
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
        public Task Handle_WhenMultipleMatchingUsersFound_ShouldThrowException()
        {
            return TestExceptionAsync(f => f.AddMultipleMatchingUsers(), f => f.Handle(), (f, r) => r.ShouldThrow<Exception>());
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
            return TestAsync(f => f.AddNonMatchingNRoles(), f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenMultipleRolesArePassedAndOnlyOneMatches_ShouldReturnTrue()
        {
            return TestAsync(f => f.SetMultipleRolesInQuery().AddSingleMatchingUser(), f => f.Handle(), (f, r) => r.Should().BeTrue());
        }
    }

    public class HasRoleQueryHandlerTestsFixture
    {
        internal HasRoleQuery Query { get; set; }
        public CancellationToken CancellationToken { get; set; }
        internal IReadStoreRequestHandler<HasRoleQuery,bool> Handler { get; set; }
        internal Mock<IAccountUsersRepository> UserRolesRepository { get; set; }
        internal List<AccountUser> Roles { get; set; }

        public HasRoleQueryHandlerTestsFixture()
        {
            Query = new HasRoleQuery(Guid.NewGuid(), 112, new HashSet<UserRole>{ UserRole.Owner });
            CancellationToken = CancellationToken.None;
            UserRolesRepository = new Mock<IAccountUsersRepository>();
            Roles = new List<AccountUser>();
            UserRolesRepository.SetupInMemoryCollection(Roles);

            Handler = new HasRoleQueryHandler(UserRolesRepository.Object);
        }

        internal Task<bool> Handle()
        {
            return Handler.Handle(Query, CancellationToken);
        }

        public HasRoleQueryHandlerTestsFixture SetMultipleRolesInQuery()
        {
            Query.UserRoles.ToList().Add(UserRole.Transactor);

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddSingleMatchingUser()
        {
            Roles.Add(CreateBasicMatchingUserRolesWithOwnerRole()); //matching

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddMultipleMatchingUsers()
        {
            Roles.Add(CreateBasicMatchingUserRolesWithOwnerRole());
            Roles.Add(CreateBasicMatchingUserRolesWithOwnerRole());

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddNonMatchingOnUserRef()
        {
            Roles.Add(CreateBasicMatchingUserRolesWithOwnerRole().Set(x => x.UserRef, Guid.NewGuid()));

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddNonMatchingOnAccountId()
        {
            Roles.Add(CreateBasicMatchingUserRolesWithOwnerRole().Set(x=>x.AccountId, 214));

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddNonMatchingOnRoleEnum()
        {
            Roles.Add(CreateBasicMatchingUserRolesObject().Set(x=>x.Role, UserRole.Transactor)); //not matching on role

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddNonMatchingNRoles()
        {
            Roles.Add(CreateBasicMatchingUserRolesObject()); //not matching no roles

            return this;
        }

        private AccountUser CreateBasicMatchingUserRolesWithOwnerRole()
        {
            return CreateBasicMatchingUserRolesObject().Set(x => x.Role, UserRole.Owner);
        }

        private AccountUser CreateBasicMatchingUserRolesObject()
        {
            return ObjectActivator.CreateInstance<AccountUser>()
                .Set(x => x.UserRef, Query.UserRef)
                .Set(x => x.AccountId, Query.AccountId);
        }
    }
}
