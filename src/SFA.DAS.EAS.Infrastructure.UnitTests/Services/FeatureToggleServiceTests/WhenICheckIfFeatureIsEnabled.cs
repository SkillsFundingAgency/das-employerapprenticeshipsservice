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
    public class WhenICheckIfFeatureIsEnabled
    {
        private const string ControllerName = "Foo";
        private const string ActionName = "Bar";
        private const string UserEmail = "user.one@unit.tests";

        private Mock<ICacheProvider> _cacheProvider;
        private Mock<FeatureToggleService> _featureToggleServiceMock;
        private FeatureToggleService _featureToggleService;

        [SetUp]
        public void Arrange()
        {
            _cacheProvider = new Mock<ICacheProvider>();

            _cacheProvider.SetupSequence(c => c.Get<FeatureToggleConfiguration>(nameof(FeatureToggleConfiguration))).Returns(null).Returns(new FeatureToggleConfiguration());

            _featureToggleServiceMock = new Mock<FeatureToggleService>(_cacheProvider.Object, Mock.Of<ILog>());

            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());
            _featureToggleServiceMock.Setup(f => f.IsFeatureEnabled(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).CallBase();

            _featureToggleService = _featureToggleServiceMock.Object;
        }

        [Test]
        public void ThenShouldGetFeaturesFromStorageOnce()
        {
            //Act
            _featureToggleService.IsFeatureEnabled("", "", null);
            _featureToggleService.IsFeatureEnabled("", "", null);

            //Assert
            _featureToggleServiceMock.Verify(f => f.GetDataFromTableStorage(), Times.Once());
        }

        [Test]
        public void ThenShouldGetFeaturesFromCache()
        {
            //Act
            _featureToggleService.IsFeatureEnabled("", "", null);
            _featureToggleService.IsFeatureEnabled("", "", null);

            //Assert
            _cacheProvider.Verify(c => c.Get<FeatureToggleConfiguration>(nameof(FeatureToggleConfiguration)), Times.Exactly(2));
        }

        [Test]
        public void ThenShouldNotCacheFeatureIfEmpty()
        {
            //Arrange
            _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());

            //Act
            _featureToggleService.IsFeatureEnabled("", "", null);

            //Assert
            _cacheProvider.Verify(c => c.Set(nameof(FeatureToggleConfiguration),It.IsAny<FeatureToggleConfiguration>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [TestCase("user\\.one@unit\\.tests")]
        [TestCase("USER\\.ONE@UNIT\\.TESTS")]
        [TestCase(".*@unit\\.tests")]
        public void ThenShouldReturnTrueIfFeatureIsDisabledButUserIsInWhitelist(string whitelistPattern)
        {
            // Arrange
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

            // Act
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, UserEmail);

            // Assert
            Assert.That(isFeatureEnabled, Is.True);
        }

        [Test]
        public void ThenShouldReturnFalseIfFeatureIsDisabledAndUserIsNotInWhitelist()
        {
            // Arrange
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

            // Act
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, UserEmail);

            // Assert
            Assert.That(isFeatureEnabled, Is.False);
        }

        [Test]
        public void ThenShouldReturnFalseIfFeatureIsDisabled()
        {
            // Arrange
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

            // Act
            var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, UserEmail);

            // Assert
            Assert.That(isFeatureEnabled, Is.False);
        }
    }
}
