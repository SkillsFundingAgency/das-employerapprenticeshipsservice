using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests
{
    class WhenISearchThePensionRegulator
    {
        private SearchPensionRegulatorController _controller;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<SearchPensionRegulatorOrchestrator> _orchestrator;
        private Mock<IMultiVariantTestingService> _multiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        private void Arrange()
        {
            _controller = new SearchPensionRegulatorController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _multiVariantTestingService.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>())
            {

            };
        }
    }
}
