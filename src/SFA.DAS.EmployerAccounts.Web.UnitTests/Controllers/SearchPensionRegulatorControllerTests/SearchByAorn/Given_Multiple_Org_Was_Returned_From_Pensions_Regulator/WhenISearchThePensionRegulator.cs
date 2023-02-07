using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Commands.PayeRefData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn.Given_Multiple_Org_Was_Returned_From_Pensions_Regulator;

[TestFixture]
class WhenISearchThePensionRegulator
{
    private const string ExpectedAorn = "0123456789ABC";
    private const string ExpectedPayeRef = "000/1234567";
    private readonly string _expectedId = Guid.NewGuid().ToString();
    private SearchPensionRegulatorController _controller;
    private SearchPensionRegulatorResultsViewModel _expectedData;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _expectedData = new SearchPensionRegulatorResultsViewModel { Results = new List<PensionRegulatorDetailsViewModel> { new PensionRegulatorDetailsViewModel(), new PensionRegulatorDetailsViewModel() } };

        var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();
        var owinWrapper = new Mock<IAuthenticationService>();
        owinWrapper.Setup(x => x.GetClaimValue(ControllerConstants.UserRefClaimKeyName)).Returns(_expectedId);

        orchestrator
            .Setup(x => x.GetOrganisationsByAorn(ExpectedAorn, ExpectedPayeRef))
            .ReturnsAsync(
                new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>
                {
                    Data = _expectedData
                });

        orchestrator.Setup(x => x.GetCookieData())
            .Returns(
                new EmployerAccountData
                {
                    EmployerAccountPayeRefData = new EmployerAccountPayeRefData
                    {
                        PayeReference = "000/1234567"
                    }
                });

        _mediator = new Mock<IMediator>();
        _mediator.Setup(x => x.Send(new UpdateUserAornLockRequest(), It.IsAny<CancellationToken>()));
        _mediator.Setup(x => x.Send(It.IsAny<GetPayeSchemeInUseQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetPayeSchemeInUseResponse());
        _controller = new SearchPensionRegulatorController(
            orchestrator.Object,              
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            _mediator.Object,
            Mock.Of<ICookieStorageService<HashedAccountIdModel>>(),
            Mock.Of<IHttpContextAccessor>());
    }

    [Test]
    public async Task ThenThePayeDetailsAreSaved()
    {
        await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
        _mediator.Verify(x => x.Send(It.Is<SavePayeRefData>(y => y.PayeRefData.AORN == ExpectedAorn && y.PayeRefData.PayeReference == ExpectedPayeRef), It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task ThenThePensionRegulatorResultsPageIsDisplayed()
    {
        var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
        var viewResponse = (ViewResult) response;

        Assert.AreEqual(ControllerConstants.SearchPensionRegulatorResultsViewName, viewResponse.ViewName);
        Assert.AreSame(_expectedData, viewResponse.Model);
    }

    [Test]
    public async Task AndTheSchemeIsAlreadyInUseThenThePayeErrorPageIsDisplayed()
    {
        _mediator.Setup(x => x.Send(It.Is<GetPayeSchemeInUseQuery>(q => q.Empref == ExpectedPayeRef), It.IsAny<CancellationToken>())).ReturnsAsync(new GetPayeSchemeInUseResponse { PayeScheme = new PayeScheme() });

        var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
        var redirectResponse = (RedirectToRouteResult)response;

        Assert.AreEqual(ControllerConstants.PayeErrorActionName, redirectResponse.RouteValues["action"].ToString());
        Assert.AreEqual(ControllerConstants.EmployerAccountControllerName, redirectResponse.RouteValues["controller"].ToString());
    }
}