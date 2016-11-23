using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.ChangeTeamMemberRole;
using SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    class WhenIChangeATeamMemberRole
    {
        private Mock<IMediator> _mediator;
        private EmployerTeamOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object);
        }

        [Test]
        public async Task ThenIShouldGetBackAnUpdatedTeamMembersListWithTheCorrectSuccessMessage()
        {
            //Assign
            const string email = "test@test.com";
            const Role role = Role.Owner;
            var response = new GetAccountTeamMembersResponse();
            _mediator.Setup(x => x.SendAsync(It.IsAny<ChangeTeamMemberRoleCommand>())).ReturnsAsync(Unit.Value);
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>())).ReturnsAsync(response);

            //Act
            var result = await _orchestrator.ChangeRole("437675", email, (short)role, "37648");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
            Assert.IsNotNull(result.FlashMessage);
            Assert.AreEqual("Team member updated", result.FlashMessage.Headline);
            Assert.AreEqual($"{email} can now {RoleStrings.GetRoleDescriptionToLower(role)}", result.FlashMessage.Message);
        }

        [Test]
        public async Task ThenIShouldGetBackABadRequestIfOneIsRaised()
        {
            //Assign
            const string email = "test@test.com";
            const Role role = Role.Owner;
            
            _mediator.Setup(x => x.SendAsync(It.IsAny<ChangeTeamMemberRoleCommand>()))
                     .Throws(new InvalidRequestException(new Dictionary<string, string>()));
           
            //Act
            var result = await _orchestrator.ChangeRole("437675", email, (short)role, "37648");
            
            
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()), Times.Never);
        }

        [Test]
        public async Task ThenIShouldGetBackAnUnauthorisedRequestIfOneIsRaised()
        {
            //Assign
            const string email = "test@test.com";
            const Role role = Role.Owner;

            _mediator.Setup(x => x.SendAsync(It.IsAny<ChangeTeamMemberRoleCommand>()))
                     .Throws(new UnauthorizedAccessException());

            //Act
            var result = await _orchestrator.ChangeRole("437675", email, (short)role, "37648");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.Status);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()), Times.Never);
        }
    }
}
