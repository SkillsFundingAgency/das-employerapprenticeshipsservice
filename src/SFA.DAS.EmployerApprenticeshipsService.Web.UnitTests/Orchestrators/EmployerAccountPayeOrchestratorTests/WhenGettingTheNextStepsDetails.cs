using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Queries.GetTeamUser;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.AccountPaye;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenGettingTheNextStepsDetails
    {
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IMediator> _mediator;

        private EmployerApprenticeshipsServiceConfiguration _configuration;

        private EmployerAccountPayeOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _logger = new Mock<ILog>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTeamMemberQuery>())).ReturnsAsync(new GetTeamMemberResponse {User = new MembershipView {ShowWizard = true, RoleId = (short)Role.Owner}});

            _orchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
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
            _mediator.Verify(x=>x.SendAsync(It.Is<GetTeamMemberQuery>(c=>c.TeamMemberId.Equals(expectedUserId) && c.HashedAccountId.Equals(expectedAccountId))));

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
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTeamMemberQuery>())).ReturnsAsync(new GetTeamMemberResponse { User = new MembershipView { ShowWizard = true, RoleId = (short)Role.Transactor } });
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
