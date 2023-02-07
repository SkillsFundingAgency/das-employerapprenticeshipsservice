using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn;

[TestFixture]
class WhenIDontProvideAValidAorn
{
    private SearchPensionRegulatorController _controller;
        
    [SetUp]
    public void Setup()
    {
        _controller = new SearchPensionRegulatorController(
            Mock.Of<SearchPensionRegulatorOrchestrator>(),
            Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
            Mock.Of<IMediator>(),
            Mock.Of<ICookieStorageService<HashedAccountIdModel>>(),
            Mock.Of<IHttpContextAccessor>());
    }

    [Test]
    public async Task ThenAnErrorIsDisplayed()
    {
        var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = "SDCXDD", PayeRef = "000/EDDEFDS" });
        var viewResponse = (ViewResult)response;

        Assert.AreEqual(ControllerConstants.SearchUsingAornViewName, viewResponse.ViewName);
        var viewModel = viewResponse.Model as SearchPensionRegulatorByAornViewModel;
        Assert.AreEqual("Enter an accounts office reference number in the correct format", viewModel.AornError);
    }
}