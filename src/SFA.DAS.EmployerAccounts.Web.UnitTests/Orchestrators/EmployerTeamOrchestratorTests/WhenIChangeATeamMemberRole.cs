using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Encoding;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    class WhenIChangeATeamMemberRole
    {
        private Mock<IMediator> _mediator;
        private Mock<IAccountApiClient> _accountApiClient;        
        private Mock<IMapper> _mapper;
        private EmployerTeamOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _accountApiClient = new Mock<IAccountApiClient>();           
            _mapper = new Mock<IMapper>();

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object, Mock.Of<ICurrentDateTime>(), _accountApiClient.Object, _mapper.Object, Mock.Of<IAuthorizationService>());
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
            var result = await _orchestrator.ChangeRole("437675", email, role, "37648");

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
            var result = await _orchestrator.ChangeRole("437675", email, role, "37648");
            
            
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
            var result = await _orchestrator.ChangeRole("437675", email, role, "37648");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, result.Status);
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()), Times.Never);
        }
    }
}
