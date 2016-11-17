using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetMember;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    class WhenIGetATeamMembersDetails
    {
        private const string ExternalUserId = "123ABC";
        private const string TeamMemberEmail = "test@test.com";
        private const string HashedAccountId = "ABC123";

        private Mock<IMediator> _mediator;
        private EmployerTeamOrchestrator _orchestrator;
        private GetMemberResponse _teamMemberResponse;
       
        [SetUp]
        public void Arrange()
        {
            _teamMemberResponse = new GetMemberResponse
            {
                TeamMember = new TeamMember()
                {
                    Email = "test@test.com"
                }
            };

            _mediator = new Mock<IMediator>();
            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetMemberRequest>()))
                .ReturnsAsync(_teamMemberResponse);
        }

        [TestCase(Role.Owner, HttpStatusCode.OK)]
        [TestCase(Role.Transactor, HttpStatusCode.Unauthorized)]
        [TestCase(Role.Viewer, HttpStatusCode.Unauthorized)]
        public async Task ThenOnlyOwnersShouldBeAbleToGetATeamMembersDetails(Role userRole, HttpStatusCode status)
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse {UserRole = userRole});

            //Act
            var result = await _orchestrator.GetTeamMember(HashedAccountId, TeamMemberEmail, ExternalUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetUserAccountRoleQuery>(q => 
                        q.HashedAccountId.Equals(HashedAccountId) && 
                        q.ExternalUserId.Equals(ExternalUserId))), Times.Once);

            Assert.AreEqual(status, result.Status);
        }

        [Test]
        public async Task ThenAnOwnerShouldBeAbleToSeeTeamMemberDetails()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse { UserRole = Role.Owner });

            //Act
            var result = await _orchestrator.GetTeamMember(HashedAccountId, TeamMemberEmail, ExternalUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetMemberRequest>(r => 
                        r.HashedId.Equals(HashedAccountId) &&
                        r.Email.Equals(TeamMemberEmail))), Times.Once);

            Assert.AreEqual(_teamMemberResponse.TeamMember, result.Data);
        }

        [TestCase(Role.Transactor)]
        [TestCase(Role.Viewer)]
        public async Task ThenUsersWhoAreNotOwnersShouldNotGetTeamMemberDetails(Role userRole)
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse { UserRole = userRole });

            //Act
            var result = await _orchestrator.GetTeamMember(HashedAccountId, TeamMemberEmail, ExternalUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetMemberRequest>()), Times.Never);
        }
    }
}
