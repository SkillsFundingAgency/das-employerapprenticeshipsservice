﻿using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    class WhenIHaveSelectedAnEmployer : ControllerTestBase
    {
        private EmployerAccountController _employerAccountController;
        private Mock<EmployerAccountOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private const string ExpectedRedirectUrl = "http://redirect.local.test";
        private EmployerAccountData _accountData;
        private OrchestratorResponse<EmployerAgreementViewModel> _response;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private const string HashedAccountId = "ABC123";

        [SetUp]
        public void Arrange()
        {
            base.Arrange(ExpectedRedirectUrl);

            _orchestrator = new Mock<EmployerAccountOrchestrator>();

            _owinWrapper = new Mock<IAuthenticationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            var logger = new Mock<ILog>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _employerAccountController = new EmployerAccountController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _userViewTestingService.Object,
                logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<ReturnUrlModel>>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>())
            {
                ControllerContext = _controllerContext.Object,
                Url = new UrlHelper(new RequestContext(_httpContext.Object, new RouteData()), _routes)
            };

            _accountData = new EmployerAccountData
            {
                EmployerAccountOrganisationData = new EmployerAccountOrganisationData
                { 
                OrganisationName = "Test Corp",
                OrganisationReferenceNumber = "1244454",
                OrganisationRegisteredAddress = "1, Test Street",
                OrganisationDateOfInception = DateTime.Now.AddYears(-10)
                }
            };

            _orchestrator.Setup(x => x.GetCookieData())
                       .Returns(_accountData);

            _response = new OrchestratorResponse<EmployerAgreementViewModel>()
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = new EmployerAgreementView
                    {
                        HashedAccountId = HashedAccountId
                    }
                },
                Status = HttpStatusCode.OK
            };

            _orchestrator.Setup(x => x.CreateOrUpdateAccount(It.IsAny<CreateAccountModel>(), It.IsAny<HttpContextBase>()))
                .ReturnsAsync(_response);
        }
        //TODO add EmployerAccountOrganisationController tests when created
        //[Test]
        //public void ThenIShouldSaveTheSelectedEmployerDetailsToCookies()
        //{
        //    //Assign
        //    var request = new OrganisationDetailsViewModel
        //    {
        //        ReferenceNumber = "6576585",
        //        Name = "Test Corp",
        //        DateOfInception = DateTime.Now.AddYears(-12),
        //        Address = "1, Test Street",
        //        Status = "active"
        //    };

        //    _orchestrator.Setup(x => x.GetGatewayUrl(It.IsAny<string>())).ReturnsAsync(ExpectedRedirectUrl);
        //    _orchestrator.Setup(x => x.CreateCookieData(It.IsAny<HttpContextBase>(), It.IsAny<EmployerAccountData>()));

        //    //Act
        //    _employerAccountController.GatewayInform(request);

        //    //Assert
        //    _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(),
        //        It.Is<EmployerAccountData>(data => 
        //        data.OrganisationReferenceNumber.Equals(request.ReferenceNumber) &&
        //        data.OrganisationStatus.Equals(request.Status) &&
        //        data.OrganisationName.Equals(request.Name) &&
        //        data.OrganisationDateOfInception.Equals(request.DateOfInception) &&
        //        data.OrganisationRegisteredAddress.Equals(request.Address))));
        //}

        //[Test]
        //public void ThenTheDataWillBeReadFromTheCookieIfIAmGoingBackThroughTheProcess()
        //{
        //    //Arrange
        //    var request = new EmployerAccountData
        //    {
        //        OrganisationReferenceNumber = "6576585",
        //        OrganisationName = "Test Corp",
        //        OrganisationDateOfInception = DateTime.Now.AddYears(-12),
        //        OrganisationRegisteredAddress = "1, Test Street",
        //        OrganisationStatus = "active"
        //    };
        //    _orchestrator.Setup(x => x.GetCookieData(It.IsAny<HttpContextBase>())).Returns(request);

        //    //Act
        //    _employerAccountController.GatewayInform(null);

        //    //Assert
        //    _orchestrator.Verify(x => x.CreateCookieData(It.IsAny<HttpContextBase>(),
        //        It.Is<EmployerAccountData>(data =>
        //        data.OrganisationReferenceNumber.Equals(request.OrganisationReferenceNumber) &&
        //        data.OrganisationStatus.Equals(request.OrganisationStatus) &&
        //        data.OrganisationName.Equals(request.OrganisationName) &&
        //        data.OrganisationDateOfInception.Equals(request.OrganisationDateOfInception) &&
        //        data.OrganisationRegisteredAddress.Equals(request.OrganisationRegisteredAddress))));
        //}
    }
}
