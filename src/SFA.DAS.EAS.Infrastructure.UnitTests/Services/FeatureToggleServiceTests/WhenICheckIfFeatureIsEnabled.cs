using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggleServiceTests
{
    [TestFixture]
    public class WhenICheckIfFeatureIsEnabled
    {
        private const string ControllerName = "Foo";
        private const string ActionName = "Bar";
        private static readonly string UserExternalId = Guid.NewGuid().ToString();
        private const string UserEmail = "user.one@unit.tests";

        private Mock<ICacheProvider> _cacheProvider;
        private Mock<ILog> _logger;
        private Mock<FeatureToggleService> _featureToggleServiceMock;
        private FeatureToggleService _featureToggleService;

        [SetUp]
        public void Arrange()
        {
            _cacheProvider = new Mock<ICacheProvider>();
            _logger = new Mock<ILog>();

            _cacheProvider.SetupSequence(c => c.Get<FeatureToggleConfiguration>(nameof(FeatureToggleConfiguration))).Returns(null).Returns(new FeatureToggleConfiguration());

            _featureToggleServiceMock = new Mock<FeatureToggleService>(_cacheProvider.Object, _logger.Object);

            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());
            _featureToggleServiceMock.Setup(f => f.IsFeatureEnabled(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).CallBase();

            _featureToggleService = _featureToggleServiceMock.Object;
        }

        [Test]
        public void ThenShouldGetFeaturesFromStorageOnce()
        {
            _featureToggleService.IsFeatureEnabled("", "", null, null);
            _featureToggleService.IsFeatureEnabled("", "", null, null);
            
            _featureToggleServiceMock.Verify(f => f.GetDataFromTableStorage(), Times.Once());
        }

        [Test]
        public void ThenShouldGetFeaturesFromCache()
        {
            _featureToggleService.IsFeatureEnabled("", "", null, null);
            _featureToggleService.IsFeatureEnabled("", "", null, null);

            _cacheProvider.Verify(c => c.Get<FeatureToggleConfiguration>(nameof(FeatureToggleConfiguration)), Times.Exactly(2));
        }

        [Test]
        public void ThenShouldNotCacheFeatureIfEmpty()
        {
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());
            
            _featureToggleService.IsFeatureEnabled("", "", null, null);
            
            _cacheProvider.Verify(c => c.Set(nameof(FeatureToggleConfiguration),It.IsAny<FeatureToggleConfiguration>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [TestCase("user\\.one@unit\\.tests")]
        [TestCase("USER\\.ONE@UNIT\\.TESTS")]
        [TestCase(".*@unit\\.tests")]
        public void ThenShouldReturnTrueIfFeatureIsDisabledButUserIsInWhitelist(string whitelistPattern)
        {
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(new FeatureToggleConfiguration
                {
                    Data = new List<FeatureToggle>
                    {
                        new FeatureToggle
                        {
                            Controller = ControllerName,
                            Action = ActionName,
                            Whitelist = new List<string> { whitelistPattern }
                        }
                    }
                });
            
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, UserExternalId, UserEmail);
            
            Assert.That(isFeatureEnabled, Is.True);
        }

        [Test]
        public void ThenShouldReturnFalseIfFeatureIsDisabledAndUserIsNotInWhitelist()
        {
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(new FeatureToggleConfiguration
                {
                    Data = new List<FeatureToggle>
                    {
                        new FeatureToggle
                        {
                            Controller = ControllerName,
                            Action = ActionName,
                            Whitelist = new List<string> { "different.user@somewhere.else" }
                        }
                    }
                });
            
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, UserExternalId, UserEmail);
            
            Assert.That(isFeatureEnabled, Is.False);
        }

        [Test]
        public void ThenShouldReturnFalseIfFeatureIsDisabled()
        {
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(new FeatureToggleConfiguration
                {
                    Data = new List<FeatureToggle>
                    {
                        new FeatureToggle
                        {
                            Controller = ControllerName,
                            Action = ActionName
                        }
                    }
                });
            
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, UserExternalId, UserEmail);
            
            Assert.That(isFeatureEnabled, Is.False);
        }

        [Test]
        public void ThenShouldLogTrueResultIfFeatureIsEnabled()
        {
            _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, UserExternalId, UserEmail);

            _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userExternalId '{UserExternalId}' is '{true}'."), Times.Once);
        }

        [Test]
        public void ThenShouldLogFalseResultIfFeatureIsDisabled()
        {
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(new FeatureToggleConfiguration
                {
                    Data = new List<FeatureToggle>
                    {
                        new FeatureToggle
                        {
                            Controller = ControllerName,
                            Action = ActionName
                        }
                    }
                });

            _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, UserExternalId, UserEmail);

            _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userExternalId '{UserExternalId}' is '{false}'."), Times.Once);
        }
    }
}
