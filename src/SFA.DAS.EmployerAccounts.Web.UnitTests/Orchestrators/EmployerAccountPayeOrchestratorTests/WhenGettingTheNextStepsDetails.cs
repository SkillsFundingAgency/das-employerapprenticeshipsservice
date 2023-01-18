using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenGettingTheNextStepsDetails
    {
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IMediator> _mediator;

        private EmployerAccountsConfiguration _configuration;

        private EmployerAccountPayeOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerAccountsConfiguration();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamMemberQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetTeamMemberResponse {User = new MembershipView {ShowWizard = true, Role = Role.Owner}});

            _orchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheUserIsReadFromTheQuery()
        {
            //Arrange
            var expectedUserId = "AFGV1234";
            var expectedAccountId = "789GBT";

            //Act
            await _orchestrator.GetNextStepsViewModel(expectedUserId, expectedAccountId);
            
            //Assert
            _mediator.Verify(x=>x.Send(It.Is<GetTeamMemberQuery>(c=>c.TeamMemberId.Equals(expectedUserId) && c.HashedAccountId.Equals(expectedAccountId)), It.IsAny<CancellationToken>()));

        }

        [Test]
        public async Task ThenTheModelIsPopulatedWithTheResponse()
        {
            //Arrange
            var expectedUserId = "AFGV1234";
            var expectedAccountId = "789GBT";

            //Act
            var actual = await _orchestrator.GetNextStepsViewModel(expectedUserId, expectedAccountId);

            //Assert
            Assert.IsAssignableFrom<OrchestratorResponse<PayeSchemeNextStepsViewModel>>(actual);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Data.ShowWizard);

        }

        [Test]
        public async Task ThenTheShowWizardFlagIsOnlyTrueWhenTheUserIsAnOwner()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetTeamMemberQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetTeamMemberResponse { User = new MembershipView { ShowWizard = true, Role = Role.Transactor } });
            var expectedUserId = "AFGV1234";
            var expectedAccountId = "789GBT";

            //Act
            var actual = await _orchestrator.GetNextStepsViewModel(expectedUserId, expectedAccountId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.ShowWizard);
        }
    }
}
