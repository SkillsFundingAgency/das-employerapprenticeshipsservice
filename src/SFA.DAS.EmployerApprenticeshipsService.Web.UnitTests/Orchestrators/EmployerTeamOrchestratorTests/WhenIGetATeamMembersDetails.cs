﻿using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetMember;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests
{
    class WhenIGetATeamMembersDetails
    {
        private readonly Guid _externalUserId = Guid.NewGuid();
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
                TeamMember = new TeamMember
                {
                    Email = "test@test.com"
                }
            };

            _mediator = new Mock<IMediator>();

            _orchestrator = new EmployerTeamOrchestrator(_mediator.Object, Mock.Of<ICurrentDateTime>());

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
            var result = await _orchestrator.GetTeamMember(HashedAccountId, TeamMemberEmail, _externalUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetUserAccountRoleQuery>(q => 
                        q.HashedAccountId.Equals(HashedAccountId) && 
                        q.ExternalUserId.Equals(_externalUserId))), Times.Once);

            Assert.AreEqual(status, result.Status);
        }

        [Test]
        public async Task ThenAnOwnerShouldBeAbleToSeeTeamMemberDetails()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>()))
                .ReturnsAsync(new GetUserAccountRoleResponse { UserRole = Role.Owner });

            //Act
            var result = await _orchestrator.GetTeamMember(HashedAccountId, TeamMemberEmail, _externalUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetMemberRequest>(r => 
                        r.HashedAccountId.Equals(HashedAccountId) &&
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
            await _orchestrator.GetTeamMember(HashedAccountId, TeamMemberEmail, _externalUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetMemberRequest>()), Times.Never);
        }
    }
}
