using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.RemoveTeamMember;
using SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EAS.Application.Queries.GetUser;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    class WhenIRemoveATeamMember
    {
        private Mock<IMediator> _mediator;
        private EmployerTeamOrchestrator _orchestrator;
        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object,_configuration);

            
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()))
                     .ReturnsAsync(new GetAccountTeamMembersResponse
                {
                    TeamMembers = new List<TeamMember>()
                });
        }

        [Test]
        public async Task ThenIShouldGetASuccessMessage()
        {
            //Assign
            var email = "test@test.com";
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserQuery>())).ReturnsAsync(new GetUserResponse
            {
                User = new User
                {
                    Email = email
                }
            });
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemoveTeamMemberCommand>())).ReturnsAsync(Unit.Value);

            //Act
            var result = await _orchestrator.Remove(2, "3242", "32342");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
            Assert.AreEqual("Team member removed", result.FlashMessage.Headline);
            Assert.AreEqual($"You've removed <strong>{email}</strong>", result.FlashMessage.Message);
        }

        [Test]
        public async Task ThenIShouldGetANotFoundErrorMessageIfNoUserCanBeFound()
        {
            //Assign
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserQuery>())).ReturnsAsync(new GetUserResponse());
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemoveTeamMemberCommand>())).ReturnsAsync(Unit.Value);

            //Act
            var result = await _orchestrator.Remove(2, "3242", "32342");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, result.Status);
        }

        [Test]
        public async Task ThenIShouldGetAInvalidRequestErrorMessageIfExceptionIsThrow()
        {
            //Assign
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserQuery>())).ReturnsAsync(new GetUserResponse {User = new User()});
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemoveTeamMemberCommand>())).Throws(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var result = await _orchestrator.Remove(2, "3242", "32342");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }

        [Test]
        public async Task ThenIShouldGetAUnauthorisedErrorMessageIfExceptionIsThrow()
        {
            //Assign
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserQuery>())).ReturnsAsync(new GetUserResponse { User = new User() });
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemoveTeamMemberCommand>())).Throws<UnauthorizedAccessException>();

            //Act
            var result = await _orchestrator.Remove(2, "3242", "32342");

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.Status);
        }
    }
}
