using System;
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
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.SearchPensionRegulatorOrchestratorTests;

[ExcludeFromCodeCoverage]
public class WhenISearchThePensionRegulatorByAorn
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
    public async Task ThenMatchingOrganisationsAreReturned()
    {
        //Arrange
        var payeRef = "123/4567";
        var aorn = "aorn";
        var queryResponse = new GetOrganisationsByAornResponse { Organisations = new List<Organisation> { new Organisation { Address = new Address() }, new Organisation { Address = new Address() } } };

        _mediator.Setup(x => x.Send(It.Is<GetOrganisationsByAornRequest>(c => c.PayeRef.Equals(payeRef) && c.Aorn.Equals(aorn)), It.IsAny<CancellationToken>())).ReturnsAsync(queryResponse);

        //Act
        var response = await _orchestrator.GetOrganisationsByAorn(aorn, payeRef);

        //Assert
        Assert.AreEqual(payeRef, response.Data.PayeRef);
        Assert.AreEqual(aorn, response.Data.Aorn);
        Assert.AreEqual(queryResponse.Organisations.Count(), response.Data.Results.Count);
    }

    [Test]
    public async Task IfGettingOrganisationsFailsThenNoOrganisationsAreReturned()
    {
        //Arrange
        var payeRef = "123/4567";
        var aorn = "aorn";
            
        _mediator.Setup(x => x.Send(It.Is<GetOrganisationsByAornRequest>(c => c.PayeRef.Equals(payeRef) && c.Aorn.Equals(aorn)), It.IsAny<CancellationToken>())).Throws(new Exception());

        //Act
        var response = await _orchestrator.GetOrganisationsByAorn(aorn, payeRef);

        //Assert
        Assert.AreEqual(payeRef, response.Data.PayeRef);
        Assert.AreEqual(aorn, response.Data.Aorn);
        Assert.AreEqual(0, response.Data.Results.Count);
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