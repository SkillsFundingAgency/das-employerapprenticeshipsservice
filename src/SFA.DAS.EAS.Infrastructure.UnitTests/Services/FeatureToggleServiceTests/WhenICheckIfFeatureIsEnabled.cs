using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggleServiceTests
{
    [TestFixture]
    public class WhenICheckIfFeatureIsEnabled
    {
        private const string ControllerName = "Foo";
        private const string ActionName = "Bar";

        private Mock<ICacheProvider> _cacheProvider;
        private Mock<ILog> _logger;
        private Mock<FeatureToggleService> _featureToggleServiceMock;
        private FeatureToggleService _featureToggleService;
        private IAuthorizationContext _authorizationContext;

        [SetUp]
        public void Arrange()
        {
            _authorizationContext = new AuthorizationContext
            {
                UserContext = new UserContext { Id = 111111, Email = "user.one@unit.tests" }
            };

            _cacheProvider = new Mock<ICacheProvider>();
            _logger = new Mock<ILog>();

            _cacheProvider.SetupSequence(c => c.Get<FeatureToggleCache>(nameof(FeatureToggleCache))).Returns(null).Returns(new FeatureToggleCache(null));

            _featureToggleServiceMock = new Mock<FeatureToggleService>(_cacheProvider.Object, _logger.Object);

            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());
            _featureToggleServiceMock.Setup(f => f.IsFeatureEnabled(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IAuthorizationContext>())).CallBase();

            _featureToggleService = _featureToggleServiceMock.Object;
        }

        [Test]
        public void ThenShouldGetFeaturesFromStorageOnce()
        {
            //Act
            _featureToggleService.IsFeatureEnabled("", "", new AuthorizationContext());

            //Assert
            _featureToggleServiceMock.Verify(f => f.GetDataFromTableStorage(), Times.Once());
        }

        [Test]
        public void ThenShouldGetFeaturesFromCache()
        {
            //Act
            _featureToggleService.IsFeatureEnabled("", "", new AuthorizationContext());
            _featureToggleService.IsFeatureEnabled("", "", new AuthorizationContext());

            //Assert
            _cacheProvider.Verify(c => c.Get<FeatureToggleCache>(nameof(FeatureToggleCache)), Times.Exactly(2));
        }

        [Test]
        public void ThenShouldCacheFeatureForAShortTimeIfEmpty()
        {
            //Arrange
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());

            //Act
            _featureToggleService.IsFeatureEnabled("", "", new AuthorizationContext());

            //Assert
            _cacheProvider.Verify(c => c.Set(nameof(FeatureToggleCache),It.IsAny<FeatureToggleCache>(), FeatureToggleService.ShortLivedCacheTime), Times.Once);
        }

        [TestCase("user\\.one@unit\\.tests")]
        [TestCase("USER\\.ONE@UNIT\\.TESTS")]
        [TestCase(".*@unit\\.tests")]
        public void ThenShouldReturnTrueIfFeatureIsDisabledButUserIsInWhitelist(string whitelistPattern)
        {
            // Arrange
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(Build(whitelistPattern));

            // Act
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _authorizationContext);

            // Assert
            Assert.That(isFeatureEnabled, Is.True);
        }

        [Test]
        public void ThenShouldReturnFalseIfFeatureIsDisabledAndUserIsNotInWhitelist()
        {
            // Arrange
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(Build("different.user@somewhere.else"));

            // Act
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _authorizationContext);

            // Assert
            Assert.That(isFeatureEnabled, Is.False);
        }

        [Test]
        public void ThenShouldReturnFalseIfFeatureIsDisabled()
        {
            // Arrange
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(Build(null));

            // Act
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _authorizationContext);

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
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext);
        }

        [Test]
        public void ThenShouldLogTrueResultIfFeatureIsEnabled()
        {
            // Act
            _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _authorizationContext);

            // Assert
            _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{_authorizationContext.UserContext.Id}' is '{true}'."), Times.Once);
        }

        [Test]
        public void ThenShouldLogFalseResultIfFeatureIsDisabled()
        {
            // Arrange
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(Build(null));

            // Act
            _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _authorizationContext);

            // Assert
            _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{_authorizationContext.UserContext.Id}' is '{false}'."), Times.Once);
        }

        [Test]
        public void ThenShouldLogNullUserExternalIdIfMembershipIsNull()
        {
            // Act
            _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, new AuthorizationContext());

            // Assert
            _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{null}' is '{true}'."), Times.Once);
        }
    }
}
