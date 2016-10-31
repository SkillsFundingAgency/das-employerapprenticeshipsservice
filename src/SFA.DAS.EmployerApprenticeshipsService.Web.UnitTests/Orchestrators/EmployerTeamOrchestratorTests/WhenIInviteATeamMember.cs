using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.CreateInvitation;
using SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    class WhenIInviteATeamMember
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
            var request = new InviteTeamMemberViewModel()
            {
                Email = "test@test.com"
            };
            var response = new GetAccountTeamMembersResponse();
            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateInvitationCommand>())).ReturnsAsync(Unit.Value);
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>())).ReturnsAsync(response);

            //Act
            var result = await _orchestrator.InviteTeamMember(request, "37648");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
            Assert.IsNotNull(result.FlashMessage);
            Assert.AreEqual("Invitation sent", result.FlashMessage.Headline);
            Assert.AreEqual($"You've sent an invitation to <strong>{request.Email}</strong>", result.FlashMessage.Message);
        }

        [Test]
        public async Task ThenIShouldGetBackABadRequestIfOneIsRaiseForInvitingTeamMembers()
        {
            //Assign
            var request = new InviteTeamMemberViewModel
            {
                Email = "test@test.com"
            };
            
            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateInvitationCommand>()))
                     .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()));

            //Act
            var result = await _orchestrator.InviteTeamMember(request, "37648");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()), Times.Never);
        }

        [Test]
        public async Task ThenIShouldGetBackAnUnauthorisedRequestIfOneIsRaiseForInvitingTeamMembers()
        {
            //Assign
            var request = new InviteTeamMemberViewModel
            {
                Email = "test@test.com"
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateInvitationCommand>()))
                     .ThrowsAsync(new UnauthorizedAccessException());

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()));

            //Act
            var result = await _orchestrator.InviteTeamMember(request, "37648");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.Status);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()), Times.Never);
        }

        [Test]
        public async Task ThenIShouldGetBackABadRequestIfOneIsRaiseForViewingTeamMembers()
        {
            //Assign
            var request = new InviteTeamMemberViewModel
            {
                Email = "test@test.com"
            };
          
            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateInvitationCommand>())).ReturnsAsync(Unit.Value);
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()))
                     .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var result = await _orchestrator.InviteTeamMember(request, "37648");

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }
    }
}
