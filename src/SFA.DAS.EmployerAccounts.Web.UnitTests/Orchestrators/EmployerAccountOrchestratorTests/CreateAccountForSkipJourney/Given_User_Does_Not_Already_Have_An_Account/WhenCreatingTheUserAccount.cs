using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests.CreateAccountForSkipJourney.Given_User_Does_Not_Already_Have_An_Account;

public class WhenCreatingTheUserAccount
{
    private EmployerAccountOrchestrator _employerAccountOrchestrator;
    private Mock<IMediator> _mediator;
    private Mock<ILogger<EmployerAccountOrchestrator>> _logger;
    private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
    private EmployerAccountsConfiguration _configuration;

    [SetUp]
    public void Arrange()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<EmployerAccountOrchestrator>>();
        _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
        _configuration = new EmployerAccountsConfiguration();

        _employerAccountOrchestrator = new EmployerAccountOrchestrator(
            _mediator.Object, 
            _logger.Object,
            _cookieService.Object, 
            _configuration,
            Mock.Of<IEncodingService>());

        _mediator.Setup(x => x.Send(It.IsAny<GetUserAccountsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserAccountsQueryResponse()
            {
                Accounts = new Accounts<Account>()
            });


        _mediator.Setup(x => x.Send(It.IsAny<CreateUserAccountCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateUserAccountCommandResponse()
            {
                HashedAccountId = "ABS10"
            });
    }

    [Test]
    public async Task ThenTheUserAccountIsCreatedWithTheCorrectValues()
    {
        //Arrange
        var model = ArrangeModel();

        //Act
        await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(model, It.IsAny<HttpContext>());

        //Assert
        _mediator.Verify(x => x.Send(It.Is<CreateUserAccountCommand>(
            c => c.OrganisationName.Equals(model.OrganisationName)
        ), It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task ThenIShouldGetBackTheNewAccountId()
    {
        //Assign
        const string hashedId = "1AFGG0";

        _mediator.Setup(x => x.Send(It.IsAny<CreateUserAccountCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateUserAccountCommandResponse()
            {
                HashedAccountId = hashedId
            });

        //Act
        var response =
            await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(new CreateUserAccountViewModel(),
                It.IsAny<HttpContext>());

        //Assert
        Assert.AreEqual(hashedId, response.Data?.HashedId);
    }

    private static CreateUserAccountViewModel ArrangeModel()
    {
        return new CreateUserAccountViewModel
        {
            OrganisationName = "test",
            UserId = Guid.NewGuid().ToString()
        };
    }
}