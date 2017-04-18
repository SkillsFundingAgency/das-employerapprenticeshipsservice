using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.FeatureToggle;
using SFA.DAS.EAS.Domain.Models.UserView;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.BaseControllerTests
{
    public class WhenOnActionExecuting : ControllerTestBase
    {
        private const string UserEmail = "user.one@unit.tests";

        private Mock<IFeatureToggle> _featureToggle;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IMultiVariantTestingService> _multiVariantTestingService;
        private TestController _controller;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

        [SetUp]
        public void Arrange()
        {
            base.Arrange();

            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(x => x.GetClaimValue("email"))
                .Returns(UserEmail);

            _featureToggle = new Mock<IFeatureToggle>();
            _featureToggle.Setup(x => x.GetFeatures())
                .Returns(new FeatureToggleLookup { Data = new List<FeatureToggleItem>() });

            _multiVariantTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            var routes = new RouteData();
            routes.Values["action"] = "TestView";
            routes.Values["controller"] = "Test";
            _controllerContext.Setup(x => x.RouteData).Returns(routes);

            _controller = new TestController(_featureToggle.Object, _owinWrapper.Object, _multiVariantTestingService.Object, _flashMessage.Object)
            {
                ControllerContext = _controllerContext.Object
            };
        }

        [Test]
        public void ThenItShouldExecuteActionIfNotInFeatureToggles()
        {
            // Act
            var actual = Invoke(() => _controller.TestView());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ContentResult>(actual);
        }

        [TestCase("user\\.one@unit\\.tests")]
        [TestCase("USER\\.ONE@UNIT\\.TESTS")]
        [TestCase(".*@unit\\.tests")]
        public void ThenItShouldExecuteActionIfToggleEnabledAndUserInWhiteList(string whitelistPattern)
        {
            // Arrange
            _featureToggle.Setup(x => x.GetFeatures())
                .Returns(new FeatureToggleLookup
                {
                    Data = new List<FeatureToggleItem>
                    {
                        new FeatureToggleItem
                        {
                            Controller = "Test",
                            Action = "TestView",
                            WhiteList = new[] { whitelistPattern }
                        }
                    }
                });
            // Act
            var actual = Invoke(() => _controller.TestView());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ContentResult>(actual);
        }

        [Test]
        public void ThenItShouldReturnFeatureNotEnabledIfToggleEnabledWithNoWhiteList()
        {
            // Arrange
            _featureToggle.Setup(x => x.GetFeatures())
                .Returns(new FeatureToggleLookup
                {
                    Data = new List<FeatureToggleItem>
                    {
                        new FeatureToggleItem
                        {
                            Controller = "Test",
                            Action = "TestView",
                            WhiteList = new string[0]
                        }
                    }
                });

            // Act
            var actual = Invoke(() => _controller.TestView());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual("FeatureNotEnabled", ((ViewResult)actual).ViewName);
        }

        [Test]
        public void ThenItShouldReturnFeatureNotEnabledIfToggleEnabledButUserNotOnWhiteList()
        {
            // Arrange
            _featureToggle.Setup(x => x.GetFeatures())
                .Returns(new FeatureToggleLookup
                {
                    Data = new List<FeatureToggleItem>
                    {
                        new FeatureToggleItem
                        {
                            Controller = "Test",
                            Action = "TestView",
                            WhiteList = new[] { "different.user@somewhere.else" }
                        }
                    }
                });

            // Act
            var actual = Invoke(() => _controller.TestView());

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual("FeatureNotEnabled", ((ViewResult)actual).ViewName);
        }
        
        [TestCase("user\\.one@unit\\.tests",true, UserEmail)]
        [TestCase("USER\\.ONE@UNIT\\.TESTS",true,UserEmail)]
        [TestCase(".*@unit\\.tests",true,UserEmail)]
        [TestCase(".*@unit\\.tests", false, "tester@testland.com")]
        public void WhenTheViewReturnedFromTheMultiVariantTestingServiceHasSplitAccessAccrossUsersToFalseThenTheViewReturnedIsFilterdByEmailAddress(string whitelistPattern, bool showDifferentView, string emailAddress)
        {
            //Arrange
            var routes = new RouteData();
            routes.Values["action"] = "TestFeatureView";
            routes.Values["controller"] = "Test";
            _controllerContext.Setup(x => x.RouteData).Returns(routes);
            _owinWrapper.Setup(x => x.GetClaimValue("email"))
                .Returns(emailAddress);
            _multiVariantTestingService.Setup(x => x.GetMultiVariantViews()).Returns(new MultiVariantViewLookup
            {
                Data = new List<MultiVariantView>
                {
                    new MultiVariantView
                    {
                        Controller = "Test",
                            Action = "TestFeatureView",
                            SplitAccessAcrossUsers = false,
                            Views = new List<ViewAccess>
                            {
                               new ViewAccess
                               {
                                   EmailAddresses =whitelistPattern.Split(',').ToList(),
                                   ViewName = "TestView_2"
                               }
                            }
                    }
                }
            });

            //Act
            var actual = Invoke(() => _controller.TestFeatureView());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
            if (showDifferentView)
            {
                
                Assert.AreEqual("TestView_2", ((ViewResult)actual).ViewName);
            }
            else
            {
                Assert.AreEqual("TestView", ((ViewResult)actual).ViewName);
            }
        }

        [Test]
        public void WhenTheViewReturnedFromTheMultiVariantTestingServiceHasSplitAccessAccrossUsersToTrueThenTheViewIsRandomlyReturned()
        {
            //Arrange
            var routes = new RouteData();
            routes.Values["action"] = "TestFeatureView";
            routes.Values["controller"] = "Test";
            _controllerContext.Setup(x => x.RouteData).Returns(routes);
            _multiVariantTestingService.Setup(x => x.GetRandomViewNameToShow(It.IsAny<List<ViewAccess>>())).Returns("someview");
            _multiVariantTestingService.Setup(x => x.GetMultiVariantViews()).Returns(new MultiVariantViewLookup
            {
                Data = new List<MultiVariantView>
                {
                    new MultiVariantView
                    {
                        Controller = "Test",
                            Action = "TestFeatureView",
                            SplitAccessAcrossUsers = true,
                            Views = new List<ViewAccess>
                            {
                               new ViewAccess
                               {
                                   ViewName = "TestView_2"
                               }
                            }
                    }
                }
            });

            //Act
            var actual = Invoke(() => _controller.TestFeatureView());

            //Assert
            _multiVariantTestingService.Verify(x=>x.GetRandomViewNameToShow(It.IsAny<List<ViewAccess>>()), Times.Once);
        }

        internal class TestController : BaseController
        {
            public TestController(IFeatureToggle featureToggle, IOwinWrapper owinWrapper, IMultiVariantTestingService multiVariantTestingService, ICookieStorageService<FlashMessageViewModel> flashMessage)
                : base(owinWrapper, featureToggle, multiVariantTestingService ,flashMessage)
            {

            }

            public ActionResult TestView()
            {
                return new ContentResult();
            }

            public ActionResult TestFeatureView()
            {
                return View("TestView",new OrchestratorResponse<string>());
            }
        }
    }
}
