using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Sections;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Pipeline.Features.Sections.WhitelistPipelineSectionTests
{
    public class WhenICheckIfAFeatureIsToggled
    {
        private const long UserId = 111111;
        private const string UserEmail = "test@test.com";
        private const string ControllerName = "TestController";
        private const string ActionName = "TestAction";

        private Mock<WhitelistPipelineSection> _whitelistSectionMock;
        private WhitelistPipelineSection _whitelistSection;
        private Mock<ICacheProvider> _cacheProvider;
        private Mock<ILog> _logger;
        private Mock<IFeatureToggleCache> _featureToggleCache;
        private ControllerActionCacheItem _actionCacheItem;
        private Mock<IMembershipContext> _membershipContext;


        [SetUp]
        public void Arrange()
        {
            _actionCacheItem = new ControllerActionCacheItem(string.Empty, string.Empty);
            _cacheProvider = new Mock<ICacheProvider>();
            _logger = new Mock<ILog>();
            _featureToggleCache = new Mock<IFeatureToggleCache>();

            _cacheProvider.Setup(x => x.Get<IFeatureToggleCache>(It.IsAny<string>()))
                          .Returns(_featureToggleCache.Object);

            _featureToggleCache.Setup(x => x.IsAvailable).Returns(true);
            _featureToggleCache.Setup(x =>
                    x.TryGetControllerActionSubjectToToggle(It.IsAny<string>(), It.IsAny<string>(),
                        out _actionCacheItem))
                .Returns(true);

            _membershipContext = new Mock<IMembershipContext>();
            _membershipContext.Setup(x => x.UserEmail).Returns(UserEmail);
            _membershipContext.Setup(x => x.UserId).Returns(UserId);

            _whitelistSectionMock = new Mock<WhitelistPipelineSection>(_cacheProvider.Object, _logger.Object);
            _whitelistSection = _whitelistSectionMock.Object;

            _whitelistSectionMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());
        }

        [Test]
        public async Task ThenItShouldBeAvailableIfUserIsOnWhitelist()
        {
            //Arrange
            _actionCacheItem.WhiteLists.Add(new WhiteList { Emails = { UserEmail } });

            //Act
            var result = await _whitelistSection.ProcessAsync(new FeatureToggleRequest
            {
                MembershipContext = _membershipContext.Object
            });

            //Arrange
            Assert.True(result);

        }

        [Test]
        public async Task ThenItShouldntBeAvailableIfUserIsNotOnWhitelist()
        {
            //Act
            var result = await _whitelistSection.ProcessAsync(new FeatureToggleRequest
            {
                MembershipContext = _membershipContext.Object
            });

            //Arrange
            Assert.False(result);
        }

        [Test]
        public async Task ThenShouldGetFeaturesFromStorageOnce()
        {
            //Arrange
            _cacheProvider.SetupSequence(x => x.Get<IFeatureToggleCache>(It.IsAny<string>()))
                .Returns(null)
                .Returns(_featureToggleCache.Object);

            //Act
            await _whitelistSection.ProcessAsync(new FeatureToggleRequest { MembershipContext = _membershipContext.Object });
            await _whitelistSection.ProcessAsync(new FeatureToggleRequest { MembershipContext = _membershipContext.Object });

            //Assert
            _whitelistSectionMock.Verify(f => f.GetDataFromTableStorage(), Times.Once());
        }

        [Test]
        public async Task ThenShouldGetFeaturesFromCache()
        {
            //Arrange
            _cacheProvider.SetupSequence(x => x.Get<IFeatureToggleCache>(It.IsAny<string>()))
                .Returns(null)
                .Returns(_featureToggleCache.Object);

            //Act
            await _whitelistSection.ProcessAsync(new FeatureToggleRequest { MembershipContext = _membershipContext.Object });
            await _whitelistSection.ProcessAsync(new FeatureToggleRequest { MembershipContext = _membershipContext.Object });

            //Assert
            _cacheProvider.Verify(c => c.Get<IFeatureToggleCache>(nameof(FeatureToggleCache)), Times.Exactly(2));
        }

        [Test]
        public async Task ThenShouldCacheFeatureForAShortTimeIfEmpty()
        {
            //Arrange
            _cacheProvider.SetupSequence(x => x.Get<IFeatureToggleCache>(It.IsAny<string>()))
                .Returns(null)
                .Returns(_featureToggleCache.Object);

            _whitelistSectionMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());
            _cacheProvider.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()));


            //Act
            await _whitelistSection.ProcessAsync(new FeatureToggleRequest { MembershipContext = _membershipContext.Object });

            //Assert
            _cacheProvider.Verify(c => c.Set(nameof(FeatureToggleCache), It.IsAny<IFeatureToggleCache>(), WhitelistPipelineSection.ShortLivedCacheTime), Times.Once);
        }

        [TestCase("user\\.one@unit\\.tests")]
        [TestCase("USER\\.ONE@UNIT\\.TESTS")]
        [TestCase(".*@unit\\.tests")]
        public async Task ThenShouldReturnTrueIfFeatureIsDisabledButUserIsInWhitelist(string whitelistPattern)
        {
            // Arrange
            _cacheProvider.SetupSequence(x => x.Get<IFeatureToggleCache>(It.IsAny<string>()))
                .Returns(null)
                .Returns(_featureToggleCache.Object);

            _whitelistSectionMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(Build(whitelistPattern));

            // Act
            var isFeatureEnabled = await _whitelistSection.ProcessAsync(new FeatureToggleRequest { MembershipContext = _membershipContext.Object });

            // Assert
            Assert.That(isFeatureEnabled, Is.True);
        }

        [Test]
        public async Task ThenShouldReturnFalseIfFeatureIsDisabledAndUserIsNotInWhitelist()
        {
            // Arrange
            _whitelistSectionMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(Build("different.user@somewhere.else"));

            // Act
            var isFeatureEnabled = await _whitelistSection.ProcessAsync(new FeatureToggleRequest { MembershipContext = _membershipContext.Object });

            // Assert
            Assert.That(isFeatureEnabled, Is.False);
        }

        [Test]
        public async Task ThenShouldReturnFalseIfFeatureIsDisabled()
        {
            // Arrange
            _whitelistSectionMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(Build(null));

            // Act
            var isFeatureEnabled =
                await _whitelistSection.ProcessAsync(new FeatureToggleRequest { MembershipContext = _membershipContext.Object });

            // Assert
            Assert.That(isFeatureEnabled, Is.False);
        }


        [Test]
        public async Task ThenShouldLogTrueResultIfFeatureIsEnabled()
        {
            //Arrange
            _cacheProvider.SetupSequence(x => x.Get<IFeatureToggleCache>(It.IsAny<string>()))
                .Returns(null)
                .Returns(_featureToggleCache.Object);

            // Act
            await _whitelistSection.ProcessAsync(new FeatureToggleRequest
            {
                Controller = ControllerName,
                Action = ActionName,
                MembershipContext = _membershipContext.Object
            });

            // Assert
            _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{UserId}' is '{true}'."), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogFalseResultIfFeatureIsDisabled()
        {
            // Arrange
            _cacheProvider.SetupSequence(x => x.Get<IFeatureToggleCache>(It.IsAny<string>()))
                .Returns(null)
                .Returns(_featureToggleCache.Object);

            _whitelistSectionMock.Setup(f => f.GetDataFromTableStorage())
                .Returns(Build(null));

            // Act
            await _whitelistSection.ProcessAsync(new FeatureToggleRequest
            {
                Controller = ControllerName,
                Action = ActionName,
                MembershipContext = _membershipContext.Object
            });

            // Assert
            _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{UserId}' is '{false}'."), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogNullUserExternalIdIfMembershipIsNull()
        {
            //Arrange
            _cacheProvider.SetupSequence(x => x.Get<IFeatureToggleCache>(It.IsAny<string>()))
                .Returns(null)
                .Returns(_featureToggleCache.Object);

            // Act
            await _whitelistSection.ProcessAsync(new FeatureToggleRequest
            {
                Controller = ControllerName,
                Action = ActionName,
                MembershipContext = null
            });

            // Assert
            _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{null}' is '{true}'."), Times.Once);
        }

        private FeatureToggleConfiguration Build(string whitelistPattern)
        {
            return new FeatureToggleConfiguration
            {
                Data = new FeatureToggleCollection
                {
                    Features = new List<FeatureToggle>
                    {
                        new FeatureToggle
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

    }
}
