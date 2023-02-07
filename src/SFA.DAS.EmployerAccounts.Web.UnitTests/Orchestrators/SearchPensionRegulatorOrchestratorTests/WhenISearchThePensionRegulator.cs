using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.SearchPensionRegulatorOrchestratorTests;

[ExcludeFromCodeCoverage]
public class WhenISearchThePensionRegulator
{
    private SearchPensionRegulatorOrchestrator _orchestrator;
    private Mock<IMediator> _mediator;
      
    [SetUp]
    public void Arrange()
    {
        _mediator = new Mock<IMediator>();
          
        _mediator.Setup(x => x.Send(It.IsAny<GetPensionRegulatorRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetPensionRegulatorResponse
            {
                Organisations = new List<Organisation>()
            });

        _orchestrator = new SearchPensionRegulatorOrchestrator(
            _mediator.Object, 
            Mock.Of<ICookieStorageService<EmployerAccountData>>(),
            Mock.Of<ILogger<SearchPensionRegulatorOrchestrator>>());
    }

    [Test]
    public async Task ThenTheMediatorIsCalledToGetThePensionRegulatorResult()
    {
        //Arrange
        var payeRef = "123/4567";
          
        //Act
        await _orchestrator.SearchPensionRegulator(payeRef);

        //Assert
        _mediator.Verify(x => x.Send(It.Is<GetPensionRegulatorRequest>(c => c.PayeRef.Equals(payeRef)), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ThenThePensionRegulatorOrganisationListIsReturnedInTheResult()
    {          
        //Act
        var actual = await _orchestrator.SearchPensionRegulator(It.IsAny<string>());

        //Assert
        Assert.IsAssignableFrom<OrchestratorResponse<SearchPensionRegulatorResultsViewModel>>(actual);
    }

    [Test]
    public async Task ThenEachResultIsCorrectlyMarkedAsComingFromPensionsRegulator()
    {
        var actual = await _orchestrator.SearchPensionRegulator(It.IsAny<string>());

        Assert
            .IsTrue(
                actual
                    .Data
                    .Results
                    .All( organisation => organisation.Type == OrganisationType.PensionsRegulator));
    }
}