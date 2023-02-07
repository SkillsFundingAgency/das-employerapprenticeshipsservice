using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.CreateAccount.Given_Cookie_Data_Is_Null;

class WhenICreateAnAccount : ControllerTestBase
{
    private EmployerAccountController _employerAccountController;
    private Mock<EmployerAccountOrchestrator> _orchestrator;
    private const string ExpectedRedirectUrl = "http://redirect.local.test";
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

    [SetUp]
    public void Arrange()
    {
        base.Arrange(ExpectedRedirectUrl);

        _orchestrator = new Mock<EmployerAccountOrchestrator>();

        var logger = new Mock<ILogger<EmployerAccountController>>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        new EmployerAccountData
        {
            EmployerAccountOrganisationData = new EmployerAccountOrganisationData
            { 
                OrganisationName = "Test Corp",
                OrganisationReferenceNumber = "1244454",
                OrganisationRegisteredAddress = "1, Test Street",
                OrganisationDateOfInception = DateTime.Now.AddYears(-10),
                OrganisationStatus = "active",
                OrganisationType = OrganisationType.Charities,
                Sector = "Public"
            },
            EmployerAccountPayeRefData = new EmployerAccountPayeRefData
            { 
                PayeReference = "123/ABC",
                EmployerRefName = "Scheme 1",
                RefreshToken = "123",
                AccessToken = "456",
                EmpRefNotFound = true,
            }
        };

        _orchestrator.Setup(x => x.GetCookieData())
            .Returns((EmployerAccountData)null);

        _employerAccountController = new EmployerAccountController(
            _orchestrator.Object,
            logger.Object,
            _flashMessage.Object,
            Mock.Of<IMediator>(),
            Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
            Mock.Of<ICookieStorageService<HashedAccountIdModel>>(),
            Mock.Of<IHttpContextAccessor>())
        {
            ControllerContext = ControllerContext.Object,
            Url = new UrlHelper(new ActionContext(HttpContext.Object, Routes, new ActionDescriptor()))
        };


    }

    [Test]
    public async Task Then_I_Should_Be_Redirected_To_Search_Organisatoin_Page()
    {
        //Act
        var result = await _employerAccountController.CreateAccount() as RedirectToRouteResult;

        //Assert
        Assert.AreEqual(ControllerConstants.SearchForOrganisationActionName, result.RouteValues["Action"]);
        Assert.AreEqual(ControllerConstants.SearchOrganisationControllerName, result.RouteValues["Controller"]);
    }

    [Test]
    public async Task Then_Orchestrator_Create_Account_Is_Not_Called()
    {
        await _employerAccountController.CreateAccount();

        _orchestrator
            .Verify(
                m => m.CreateOrUpdateAccount(It.IsAny<CreateAccountModel>(), It.IsAny<HttpContext>())
                , Times.Never);
    }
}