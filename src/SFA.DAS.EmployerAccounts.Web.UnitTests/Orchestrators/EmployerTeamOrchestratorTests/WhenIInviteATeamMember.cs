﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Commands.CreateInvitation;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    class WhenIInviteATeamMember
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
