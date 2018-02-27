using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggleServiceTests
{
    [TestFixture]
    public class WhenICheckIfFeatureIsEnabled
    {
        private const string ControllerName = "Test_Controller";
        private const string ActionName = "Test_Action";

        private Mock<IOperationAuthorisationHandler> _pipeline;
	    private Mock<IAuthorizationContext> _authorisationContext;
        private OperationAuthorisationService _operationAuthorisationService;

        [SetUp]
        public void Arrange()
        {
            _pipeline = new Mock<IOperationAuthorisationHandler>();
            _authorisationContext = new Mock<IAuthorizationContext>();
            _cacheProvider.SetupSequence(c => c.Get<FeatureToggleCache>(nameof(FeatureToggleCache))).Returns(null).Returns(new FeatureToggleCache(null));
            _cacheProvider.Verify(c => c.Get<FeatureToggleCache>(nameof(FeatureToggleCache)), Times.Exactly(2));
        public void ThenShouldCacheFeatureForAShortTimeIfEmpty()
            _cacheProvider.Verify(c => c.Set(nameof(FeatureToggleCache),It.IsAny<FeatureToggleCache>(), FeatureToggleService.ShortLivedCacheTime), Times.Once);
                .Returns(Build(whitelistPattern));
                .Returns(Build("different.user@somewhere.else"));
                .Returns(Build(null));

            // Act
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext);

            // Assert
            Assert.That(isFeatureEnabled, Is.False);
        }

        private FeatureToggleConfiguration Build(string whitelistPattern)
        {
            return new FeatureToggleConfiguration
            {
                Data = new FeatureToggleCollection
                {
                    Features = new List<Domain.Models.FeatureToggles.FeatureToggle>
                    {
                        new Domain.Models.FeatureToggles.FeatureToggle
                        {
                            Actions = new List<ControllerAction>
                            {
                                new ControllerAction
                                {
                                    Controller = ControllerName,
                                    Action = ActionName
                                }
                            },
                            Whitelist = new WhiteList {Emails = string.IsNullOrWhiteSpace(whitelistPattern) ? null : new List<string> {whitelistPattern}}
                        }
                    }
                }
            };
        }

        [Test]
        public void ThenShouldReturnThatFeatureIsEnabled()
        {
            //Act
            var result = _operationAuthorisationService.IsOperationAuthorised(ControllerName, ActionName, _authorisationContext.Object);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ThenShouldReturnThatFeatureIsNotEnabled()
        {
            //Assign
            _pipeline.Setup(x => x.CanAccessAsync(It.IsAny<OperationContext>())).ReturnsAsync(false);
                .Returns(Build(null));

            //Act
            var result = _operationAuthorisationService.IsOperationAuthorised(ControllerName, ActionName, _authorisationContext.Object);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ThenShouldRequestDetails()
        {
            //Act
            _operationAuthorisationService.IsOperationAuthorised(ControllerName, ActionName, _authorisationContext.Object);

            //Assert
            _pipeline.Verify(x => x.CanAccessAsync(It.Is<OperationContext>(
                operationContext => operationContext.Controller.Equals(ControllerName) &&
                operationContext.Action.Equals(ActionName) &&
                operationContext.AuthorisationContext == _authorisationContext.Object)), Times.Once);
        }
    }
}
