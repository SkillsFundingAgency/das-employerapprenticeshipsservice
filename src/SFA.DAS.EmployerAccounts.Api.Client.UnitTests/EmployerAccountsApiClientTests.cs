using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.Api.Client.UnitTests
{
    [TestFixture]
    [Parallelizable]
    public class EmployerAccountsApiClientTests : FluentTest<EmployerAccountsApiClientTestsFixture>
    {
        [Test]
        public Task HasRole_ShouldCallTheMediatorCorrectly()
        {
            return TestAsync(f => f.SetupHasRole(), f => f.HasRole(), f => f.VerifyMediatorCall());
        }

        [Test]
        public Task HasRole_ShouldReturnTheExpectedValue()
        {
            return TestAsync(f => f.SetupHasRole(), f => f.HasRole(), (f, r) => r.Should().BeTrue());
        }
    }

    public class EmployerAccountsApiClientTestsFixture
    {
        public HasRoleRequest HasRoleRequest { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Mock<IReadStoreMediator> MockApiMediator { get; set; }
        public IEmployerAccountsApiClient EmployerAccountsApiClient { get; set; }

        public EmployerAccountsApiClientTestsFixture()
        {
            CancellationToken = CancellationToken.None;
            MockApiMediator = new Mock<IReadStoreMediator>();
            EmployerAccountsApiClient = new EmployerAccountsApiClient(null, MockApiMediator.Object);
        }

        public EmployerAccountsApiClientTestsFixture SetupHasRole()
        {
            HasRoleRequest = new HasRoleRequest
            {
                EmployerAccountId = 112,
                UserRef = Guid.NewGuid(),
                Roles = new HashSet<UserRole> {UserRole.Owner, UserRole.Transactor}
            };

            MockApiMediator
                .Setup(m => m.Send(It.IsAny<HasRoleQuery>(), CancellationToken))
                .ReturnsAsync(true);

            return this;
        }

        public Task<bool> HasRole()
        {
            return EmployerAccountsApiClient.HasRole(HasRoleRequest, CancellationToken);
        }

        public void VerifyMediatorCall()
        {
            MockApiMediator.Verify(m => m.Send(It.Is<HasRoleQuery>(q => 
                q.UserRef == HasRoleRequest.UserRef
                && q.AccountId == HasRoleRequest.EmployerAccountId
                && q.UserRoles.Count == HasRoleRequest.Roles.Count
                && q.UserRoles.All(role => HasRoleRequest.Roles.Any(requestRole => (short)requestRole == (short)role))
            ), CancellationToken));
        }
    }
}
