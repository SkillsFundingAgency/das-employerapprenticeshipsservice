using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn
{
    [TestFixture]
    class WhenIDontProvideAPayeRef
    {
        private SearchPensionRegulatorController _controller;
        
        [SetUp]
        public void Setup()
        {
            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                Mock.Of<SearchPensionRegulatorOrchestrator>(),
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                Mock.Of<IMediator>(),
                Mock.Of<ICookieStorageService<HashedAccountIdModel>>());
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task ThenAnErrorIsDisplayed(string payeRef)
        {
            var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = "1234567890ABC", PayeRef = payeRef });
            var viewResponse = (ViewResult)response;

            Assert.AreEqual(ControllerConstants.SearchUsingAornViewName, viewResponse.ViewName);
            var viewModel = viewResponse.Model as SearchPensionRegulatorByAornViewModel;
            Assert.AreEqual("Enter your PAYE scheme to continue", viewModel.PayeRefError);
        }
    }
}
