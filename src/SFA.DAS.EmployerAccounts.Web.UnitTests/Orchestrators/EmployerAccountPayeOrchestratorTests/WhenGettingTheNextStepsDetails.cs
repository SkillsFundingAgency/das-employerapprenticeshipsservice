using AutoFixture.NUnit3;
using MediatR;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenGettingTheNextStepsDetails
    {
        [Test, MoqAutoData]
        public async Task ThenTheUserIsReadFromTheQuery(
            string userId,
            string hashedAccountId,
            long accountId,
            GetTeamMemberResponse teamMemberResponse,
            MembershipView user,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IEncodingService> encodingService,
            EmployerAccountPayeOrchestrator orchestrator)
        {
            //Arrange
            teamMemberResponse.User = user;
            mediator.Setup(x => x.Send(It.IsAny<GetTeamMemberQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(teamMemberResponse);
            encodingService.Setup(e => e.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);

            //Act
            await orchestrator.GetNextStepsViewModel(userId, hashedAccountId);

            //Assert
            mediator.Verify(x => x.Send(It.Is<GetTeamMemberQuery>(c => c.TeamMemberId.Equals(userId) && c.AccountId.Equals(accountId)), It.IsAny<CancellationToken>()));

        }

        [Test, MoqAutoData]
        public async Task ThenTheModelIsPopulatedWithTheResponse(
            string userId,
            string hashedAccountId,
            long accountId,
            GetTeamMemberResponse teamMemberResponse,
            MembershipView user,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IEncodingService> encodingService,
            EmployerAccountPayeOrchestrator orchestrator)
        {
            //Arrange
            user.Role = Role.Owner;
            user.ShowWizard = true;
            teamMemberResponse.User = user;
            mediator.Setup(x => x.Send(It.IsAny<GetTeamMemberQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(teamMemberResponse);
            encodingService.Setup(e => e.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);

            //Act
            var actual = await orchestrator.GetNextStepsViewModel(userId, hashedAccountId);

            //Assert
            Assert.IsAssignableFrom<OrchestratorResponse<PayeSchemeNextStepsViewModel>>(actual);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Data.ShowWizard);

        }

        [Test, MoqAutoData]
        public async Task ThenTheShowWizardFlagIsOnlyTrueWhenTheUserIsAnOwner(
            string userId,
            string hashedAccountId,
            long accountId,
            GetTeamMemberResponse teamMemberResponse,
            MembershipView user,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IEncodingService> encodingService,
            EmployerAccountPayeOrchestrator orchestrator)
        {
            //Arrange
            user.ShowWizard = true;
            user.Role = Role.Transactor;
            teamMemberResponse.User = user;
            mediator.Setup(x => x.Send(It.IsAny<GetTeamMemberQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(teamMemberResponse);
            encodingService.Setup(e => e.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);

            //Act
            var actual = await orchestrator.GetNextStepsViewModel(userId, hashedAccountId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.ShowWizard);
        }
    }
}
