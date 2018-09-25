using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetUser;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    class WhenIRemoveATeamMember
    {
        private Mock<IMediator> _mediator;
        private EmployerTeamOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object, Mock.Of<ICurrentDateTime>());
            
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
            const string email = "test@test.com";
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
