using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    public class WhenIGetMyTeamMembers
    {
        private Mock<IMediator> _mediator;
        private EmployerTeamOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>())).ReturnsAsync(new GetAccountTeamMembersResponse {TeamMembers = new List<TeamMember> {new TeamMember()} });

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object);
        }

        [Test]
        public async Task ThenTheGetTeamMembersCallWillShowASuccessMessageIfAnEmailHasBeenPassed()
        {
            //Arrange
            var expectedEmail = "test@test.com";

            //Act
            var actual = await _orchestrator.GetTeamMembers("ABF45", "123", expectedEmail);

            //Assert
            Assert.IsNotNull(actual.FlashMessage);
            Assert.AreEqual("Invitation sent", actual.FlashMessage.Headline);
            Assert.AreEqual($"You've sent an invitation to <strong>{expectedEmail}</strong>", actual.FlashMessage.Message);
        }

        [Test]
        public async Task ThenTheTeamMembersArePopulatedToTheResponse()
        {
            //Act
            var actual = await _orchestrator.GetTeamMembers("ABF45", "123");

            //Assert
            Assert.IsNotEmpty(actual.Data.TeamMembers);
        }
    }
}
