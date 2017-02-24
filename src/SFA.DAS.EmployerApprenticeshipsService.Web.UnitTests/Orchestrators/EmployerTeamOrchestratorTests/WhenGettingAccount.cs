using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    public class WhenGettingAccount
    {
        private const string AccountId = "ABC123";
        private const string UserId = "USER1";

        private Mock<IMediator> _mediator;
        private EmployerTeamOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetEmployerAccountHashedQuery>(q => q.HashedAccountId == AccountId)))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Domain.Data.Entities.Account.Account
                    {
                        HashedId = AccountId,
                        Id = 123,
                        Name = "Account 1"
                    }
                });
            _mediator.Setup(m => m.SendAsync(It.Is<GetUserAccountRoleQuery>(q => q.ExternalUserId == UserId)))
                .ReturnsAsync(new GetUserAccountRoleResponse
                {
                    UserRole = Domain.Models.UserProfile.Role.Owner
                });
            _mediator.Setup(m => m.SendAsync(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == AccountId)))
                .ReturnsAsync(new GetAccountEmployerAgreementsResponse
                {
                    EmployerAgreements = new List<Domain.Models.EmployerAgreement.EmployerAgreementView>
                    {
                        new Domain.Models.EmployerAgreement.EmployerAgreementView {Status = Domain.Models.EmployerAgreement.EmployerAgreementStatus.Pending}
                    }
                });

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object);
        }

        [Test]
        public async Task ThenAnAgreementShouldNeedSigningIfTheUserIsAnOwnerOrTransactorAndThereAreUnsignedAgreements()
        {
            // Act
            var actual = await _orchestrator.GetAccount(AccountId, UserId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Data.RequiresAgreementSigning);
        }

        [Test]
        public async Task ThenAnAgreementShouldNotNeedSigningIfTheUserIsAnOwnerOrTransactorButThereAreNoAgreementsThatNeedSigning()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == AccountId)))
                .ReturnsAsync(new GetAccountEmployerAgreementsResponse
                {
                    EmployerAgreements = new List<Domain.Models.EmployerAgreement.EmployerAgreementView>
                    {
                        new Domain.Models.EmployerAgreement.EmployerAgreementView {Status = Domain.Models.EmployerAgreement.EmployerAgreementStatus.Signed}
                    }
                });

            // Act
            var actual = await _orchestrator.GetAccount(AccountId, UserId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.RequiresAgreementSigning);
        }

        [Test]
        public async Task ThenAnAgreementShouldNotNeedSigningIfTheUserIsAViewer()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<GetUserAccountRoleQuery>(q => q.ExternalUserId == UserId)))
                .ReturnsAsync(new GetUserAccountRoleResponse
                {
                    UserRole = Domain.Models.UserProfile.Role.Viewer
                });

            // Act
            var actual = await _orchestrator.GetAccount(AccountId, UserId);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.RequiresAgreementSigning);
        }
    }
}
