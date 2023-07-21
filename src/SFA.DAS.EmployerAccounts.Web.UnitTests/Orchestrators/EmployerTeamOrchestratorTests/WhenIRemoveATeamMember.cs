using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetUser;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerTeamOrchestratorTests;

class WhenIRemoveATeamMember
{
    const string Email = "test@test.com";

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

        _orchestrator = new EmployerTeamOrchestrator(_mediator.Object, Mock.Of<ICurrentDateTime>(), _accountApiClient.Object, _mapper.Object, Mock.Of<EmployerAccountsConfiguration>(), Mock.Of<IEncodingService>());
            
        _mediator.Setup(x => x.Send(It.IsAny<GetAccountTeamMembersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAccountTeamMembersResponse
            {
                TeamMembers = new List<TeamMember>()
            });

        _mediator.Setup(x => x.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetUserResponse
        {
            User = new User
            {
                Email = Email,
                UserRef = Guid.NewGuid().ToString()
            }
        });
    }

    [Test]
    public async Task ThenIShouldGetASuccessMessage()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<RemoveTeamMemberCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        //Act
        var result = await _orchestrator.Remove(2, "3242", "32342");

        //Assert
        Assert.AreEqual(HttpStatusCode.OK, result.Status);
        Assert.AreEqual("Team member removed", result.FlashMessage.Headline);
        Assert.AreEqual($"You've removed <strong>{Email}</strong>", result.FlashMessage.Message);
    }

    [Test]
    public async Task ThenIShouldGetANotFoundErrorMessageIfNoUserCanBeFound()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetUserResponse());
        _mediator.Setup(x => x.Send(It.IsAny<RemoveTeamMemberCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

        //Act
        var result = await _orchestrator.Remove(2, "3242", "32342");

        //Assert
        Assert.AreEqual(HttpStatusCode.NotFound, result.Status);
    }

    [Test]
    public async Task ThenIShouldGetAInvalidRequestErrorMessageIfExceptionIsThrow()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<RemoveTeamMemberCommand>(), It.IsAny<CancellationToken>())).Throws(new InvalidRequestException(new Dictionary<string, string>()));

        //Act
        var result = await _orchestrator.Remove(2, "3242", "32342");

        //Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
    }

    [Test]
    public async Task ThenIShouldGetAUnauthorisedErrorMessageIfExceptionIsThrow()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<RemoveTeamMemberCommand>(), It.IsAny<CancellationToken>())).Throws<UnauthorizedAccessException>();

        //Act
        var result = await _orchestrator.Remove(2, "3242", "32342");

        //Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, result.Status);
    }
}