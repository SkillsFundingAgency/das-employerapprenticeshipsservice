using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Queries;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.Api.Client.UnitTests
{
    [TestFixture]
    public class EmployerAccountsApiClientTests : FluentTest<EmployerAccountsApiClientTestsFixture>
    {
        [Test]
        public Task HasRole_ShouldCallTheMediatorCorrectly()
        {
            return RunAsync(f => f.SetupHasRole(), f => f.HasRole(), f => f.VerifyMediatorCall());
        }

        [Test]
        public Task HasRole_ShouldReturnTheExpectedValue()
        {
            return RunAsync(f => f.SetupHasRole(), f => f.HasRole(), (f, r) => r.Should().BeTrue());
        }
    }

    public class EmployerAccountsApiClientTestsFixture
    {
        public HasRoleRequest HasRoleRequest { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Mock<IApiMediator> MockApiMediator { get; set; }
        public IEmployerAccountsApiClient EmployerAccountsApiClient { get; set; }

        public EmployerAccountsApiClientTestsFixture()
        {
            CancellationToken = CancellationToken.None;
            MockApiMediator = new Mock<IApiMediator>();
            EmployerAccountsApiClient = new EmployerAccountsApiClient(null, MockApiMediator.Object);
        }

        public EmployerAccountsApiClientTestsFixture SetupHasRole()
        {
            HasRoleRequest = new HasRoleRequest
            {
                EmployerAccountId = 112,
                UserRef = Guid.NewGuid(),
                Roles = new[] {Role.Owner, Role.Transactor}
            };

            MockApiMediator
                .Setup(m => m.Send(It.IsAny<HasRoleQuery>(), CancellationToken))
                .ReturnsAsync(new HasRoleQueryResult() {HasRole = true});

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
                && q.EmployerAccountId == HasRoleRequest.EmployerAccountId
                && q.UserRoles.Length == HasRoleRequest.Roles.Length
                && q.UserRoles.All(role => HasRoleRequest.Roles.Any(requestRole => (short)requestRole == (short)role))
            ), CancellationToken));
        }

    }
}
