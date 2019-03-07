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
        public Task IsUserInRole_ShouldCallTheMediatorCorrectly()
        {
            return TestAsync(f => f.IsUserInRole(), f => f.VerifyMediatorCallForIsUserInAny());
        }

        [Test]
        public Task IsUserInRole_ShouldReturnTheExpectedValue()
        {
            return TestAsync(f => f.IsUserInRole(), (f, r) => r.Should().BeTrue());
        }

        [Test]
        public Task IsUserInAnyRole_ShouldCallTheMediatorCorrectly()
        {
            return TestAsync(f => f.IsUserInRole(), f => f.VerifyMediatorCallForIsUserInAny());
        }

        [Test]
        public Task IsUserInAnyRole_ShouldReturnTheExpectedValue()
        {
            return TestAsync(f => f.IsUserInRole(), (f, r) => r.Should().BeTrue());
        }
    }

    public class EmployerAccountsApiClientTestsFixture
    {
        public IsUserInRoleRequest IsUserInRoleRequest { get; set; }
        public IsUserInAnyRoleRequest IsUserInAnyRoleRequest { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Mock<IReadStoreMediator> MockApiMediator { get; set; }
        public IEmployerAccountsApiClient EmployerAccountsApiClient { get; set; }

        public EmployerAccountsApiClientTestsFixture()
        {
            CancellationToken = CancellationToken.None;
            MockApiMediator = new Mock<IReadStoreMediator>();
            EmployerAccountsApiClient = new EmployerAccountsApiClient(null, MockApiMediator.Object);

            IsUserInRoleRequest = new IsUserInRoleRequest
            {
                AccountId = 112,
                UserRef = Guid.NewGuid(),
                Roles = new HashSet<UserRole> {UserRole.Owner, UserRole.Transactor}
            };

            MockApiMediator
                .Setup(m => m.Send(It.IsAny<IsUserInRoleQuery>(), CancellationToken))
                .ReturnsAsync(true);

            IsUserInAnyRoleRequest = new IsUserInAnyRoleRequest
            {
                AccountId = 112,
                UserRef = Guid.NewGuid()
            };

            MockApiMediator
                .Setup(m => m.Send(It.IsAny<IsUserInAnyRoleQuery>(), CancellationToken))
                .ReturnsAsync(true);
        }

        public Task<bool> IsUserInRole()
        {
            return EmployerAccountsApiClient.IsUserInRole(IsUserInRoleRequest, CancellationToken);
        }

        public Task<bool> IsUserInAnyRole()
        {
            return EmployerAccountsApiClient.IsUserInAnyRole(IsUserInAnyRoleRequest, CancellationToken);
        }

        public void VerifyMediatorCallForIsUserInAny()
        {
            MockApiMediator.Verify(m => m.Send(It.Is<IsUserInRoleQuery>(q => 
                q.UserRef == IsUserInRoleRequest.UserRef
                && q.AccountId == IsUserInRoleRequest.AccountId
                && q.UserRoles.Count == IsUserInRoleRequest.Roles.Count
                && q.UserRoles.All(role => IsUserInRoleRequest.Roles.Any(requestRole => requestRole == role))
            ), CancellationToken));
        }
    }
}
