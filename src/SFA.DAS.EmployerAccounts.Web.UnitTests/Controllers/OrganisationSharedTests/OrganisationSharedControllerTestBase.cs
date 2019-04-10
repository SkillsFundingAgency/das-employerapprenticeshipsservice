using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationSharedTests
{
    public abstract class OrganisationSharedControllerTestBase
    {
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        protected OrganisationSharedController Controller;
        protected Mock<OrganisationOrchestrator> Orchestrator;
        protected Mock<IMapper> Mapper;

        [SetUp]
        public virtual void Arrange()
        {
            Orchestrator = new Mock<OrganisationOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            Mapper = new Mock<IMapper>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            SetupOrchestrator();
            SetupMapper();

            _logger = new Mock<ILog>();

            Controller = new OrganisationSharedController(
                _owinWrapper.Object,
                Orchestrator.Object,
                _featureToggle.Object,
                _userViewTestingService.Object,
                Mapper.Object,
                _logger.Object,
                _flashMessage.Object,
                Mock.Of<IMediator>());
        }

        public virtual void SetupOrchestrator()
        {
            
        }

        public virtual void SetupMapper()
        {
            
        }

        protected void SetupPopulatedRouteData()
        {
            var routeData = new RouteData();
            routeData.Values.Add(ControllerConstants.AccountHashedIdRouteKeyName, "ABC123");
            Controller.ControllerContext = new ControllerContext(new Mock<HttpContextBase>().Object, routeData, Controller);
        }

        protected void SetupEmptyRouteData()
        {
            var routeData = new RouteData();
            Controller.ControllerContext = new ControllerContext(new Mock<HttpContextBase>().Object, routeData, Controller);
        }
    }
}
