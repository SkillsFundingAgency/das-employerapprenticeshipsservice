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
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

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
            //Arrange
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
            
        }

        [Test]
        public async Task ThenIShouldGetBackABadRequestIfOneIsRaiseForInvitingTeamMembers()
        {
            //Arrange
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
            //Arrange
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
        
        
        [TestCase(Role.Viewer, HttpStatusCode.Unauthorized)]
        [TestCase(Role.Transactor, HttpStatusCode.Unauthorized)]
        [TestCase(Role.Owner, HttpStatusCode.OK)]
        public async Task ThenIShouldNotBeAllowedToGetANewInvitationIfIAmNotAnOwnerOfTheAccount(Role userRole, HttpStatusCode status)
        {
            //Arrange
            const string hashAccountId = "123ABC";
            const string externalUserId = "1234";

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                     .ReturnsAsync(new GetUserAccountRoleResponse
                     {
                         UserRole = userRole
                     });

            //Act
            var result = await _orchestrator.GetNewInvitation(hashAccountId, externalUserId);

            //Assert
            Assert.AreEqual(status, result.Status);
        }
        
    }
}
