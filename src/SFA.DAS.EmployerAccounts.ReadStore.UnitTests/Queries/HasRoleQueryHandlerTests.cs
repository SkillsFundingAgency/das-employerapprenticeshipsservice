using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.ReadStore.Queries;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Queries
{
    [TestFixture]
    [Parallelizable]
    public class HasRoleQueryHandlerTests : FluentTest<HasRoleQueryHandlerTestsFixture>
    {
        [Test]
        public Task Handle_ShouldReturnTrueOnMatch()
        {
            return RunAsync(f => f.AddSingleMatchingUserRole(), f => f.Handle(), (f, r) => r.HasRole.Should().BeTrue());
        }

        [Test]
        public Task Handle_ShouldReturnFalseWhenNotMatchingBecauseOfNonMatchingUserRef()
        {
            return RunAsync(f => f.AddNonMatchingOnUserRefRole(), f => f.Handle(), (f, r) => r.HasRole.Should().BeFalse());
        }

        [Test]
        public Task Handle_ShouldReturnFalseWhenNotMatchingBecauseOfNonMatchingAccountId()
        {
            return RunAsync(f => f.AddNonMatchingOnAccountIdRole(), f => f.Handle(), (f, r) => r.HasRole.Should().BeFalse());
        }

        [Test]
        public Task Handle_ShouldReturnFalseWhenNotMatchingBecauseOfNonMatchingRoleEnum()
        {
            return RunAsync(f => f.AddNonMatchingOnRoleEnumRole(), f => f.Handle(), (f, r) => r.HasRole.Should().BeFalse());
        }

        [Test]
        public Task Handle_ShouldReturnTrueWhenMultipleRolesArePassedAndOnlyOneMatches()
        {
            return RunAsync(f => f.SetMultipleRolesInQuery().AddSingleMatchingUserRole(), f => f.Handle(), (f, r) => r.HasRole.Should().BeTrue());
        }
    }

    public class HasRoleQueryHandlerTestsFixture
    {
        internal HasRoleQuery Query { get; set; }
        public CancellationToken CancellationToken { get; set; }
        internal IReadStoreRequestHandler<HasRoleQuery,HasRoleQueryResult> Handler { get; set; }
        internal Mock<IUsersRolesRepository> MockUserRolesRepository { get; set; }
        internal IOrderedQueryable<UserRoles> DocumentQuery { get; set; }
        internal List<UserRoles> Roles { get; set; }

        public HasRoleQueryHandlerTestsFixture()
        {
            Query = new HasRoleQuery(Guid.NewGuid(), 112, new HashSet<UserRole>{ UserRole.Owner });
            CancellationToken = CancellationToken.None;
            MockUserRolesRepository = new Mock<IUsersRolesRepository>();
            Roles = new List<UserRoles>();
            DocumentQuery = new FakeDocumentQuery<UserRoles>(Roles);

            MockUserRolesRepository.Setup(r => r.CreateQuery(null)).Returns(DocumentQuery);

            Handler = new HasRoleQueryHandler(MockUserRolesRepository.Object);
        }

        internal Task<HasRoleQueryResult> Handle()
        {
            return Handler.Handle(Query, CancellationToken);
        }

        public HasRoleQueryHandlerTestsFixture SetMultipleRolesInQuery()
        {
            Query.UserRoles.ToList().Add(UserRole.Transactor);

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddSingleMatchingUserRole()
        {
            Roles.AddRange(new []{
                new UserRoles(Query.UserRef, Query.EmployerAccountId, Query.UserRoles, DateTime.UtcNow), //matching
            });

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddNonMatchingOnUserRefRole()
        {
            Roles.AddRange(new[]{
                new UserRoles(Guid.NewGuid(), Query.EmployerAccountId, Query.UserRoles, DateTime.UtcNow), //not matching on UserRef
            });

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddNonMatchingOnAccountIdRole()
        {
            Roles.AddRange(new[]{
                new UserRoles(Query.UserRef, 214, Query.UserRoles, DateTime.UtcNow), //not matching on account id
            });

            return this;
        }

        public HasRoleQueryHandlerTestsFixture AddNonMatchingOnRoleEnumRole()
        {
            Roles.AddRange(new[]{
                new UserRoles(Query.UserRef, Query.EmployerAccountId, new HashSet<UserRole>{ UserRole.Transactor}, DateTime.UtcNow), //not matching on role
            });

            return this;
        }
    }
}
