using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Pipeline.Features.Sections;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Pipeline.Features.Sections.WhitelistPipelineSectionTests
{
    public class WhenICheckIfAFeatureIsToggled
    {
        private const string UserEmail = "test@test.com";

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

            _whitelistSection = new WhitelistPipelineSection(_cacheProvider.Object, _logger.Object);

            //_membershipContext = new MembershipContext
            //{
            //    UserId = 111111,
            //    UserEmail = "user.one@unit.tests"
            //};

            //_cacheProvider = new Mock<ICacheProvider>();
            //_logger = new Mock<ILog>();

            //_cacheProvider.SetupSequence(c => c.Get<FeatureToggleCache>(nameof(FeatureToggleCache))).Returns(null).Returns(new FeatureToggleCache(null));

            //_featureToggleServiceMock = new Mock<FeatureToggleService>(_cacheProvider.Object, _logger.Object);

            //_featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());
            //_featureToggleServiceMock.Setup(f => f.IsFeatureEnabled(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IMembershipContext>())).CallBase();

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

        //[Test]
        //public void ThenShouldGetFeaturesFromStorageOnce()
        //{
        //    //Act
        //    _featureToggleService.IsFeatureEnabled("", "", null);
        //    _featureToggleService.IsFeatureEnabled("", "", null);

        //    //Assert
        //    _featureToggleServiceMock.Verify(f => f.GetDataFromTableStorage(), Times.Once());
        //}

        //[Test]
        //public void ThenShouldGetFeaturesFromCache()
        //{
        //    //Act
        //    _featureToggleService.IsFeatureEnabled("", "", null);
        //    _featureToggleService.IsFeatureEnabled("", "", null);

        //    //Assert
        //    _cacheProvider.Verify(c => c.Get<FeatureToggleCache>(nameof(FeatureToggleCache)), Times.Exactly(2));
        //}

        //[Test]
        //public void ThenShouldCacheFeatureForAShortTimeIfEmpty()
        //{
        //    //Arrange
        //    _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage()).Returns(new FeatureToggleConfiguration());

        //    //Act
        //    _featureToggleService.IsFeatureEnabled("", "", null);

        //    //Assert
        //    _cacheProvider.Verify(c => c.Set(nameof(FeatureToggleCache), It.IsAny<FeatureToggleCache>(), FeatureToggleService.ShortLivedCacheTime), Times.Once);
        //}

        //[TestCase("user\\.one@unit\\.tests")]
        //[TestCase("USER\\.ONE@UNIT\\.TESTS")]
        //[TestCase(".*@unit\\.tests")]
        //public void ThenShouldReturnTrueIfFeatureIsDisabledButUserIsInWhitelist(string whitelistPattern)
        //{
        //    // Arrange
        //    _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
        //        .Returns(Build(whitelistPattern));

        //    // Act
        //    var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext);

        //    // Assert
        //    Assert.That(isFeatureEnabled, Is.True);
        //}

        //[Test]
        //public void ThenShouldReturnFalseIfFeatureIsDisabledAndUserIsNotInWhitelist()
        //{
        //    // Arrange
        //    _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
        //        .Returns(Build("different.user@somewhere.else"));

        //    // Act
        //    var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext);

        //    // Assert
        //    Assert.That(isFeatureEnabled, Is.False);
        //}

        //[Test]
        //public void ThenShouldReturnFalseIfFeatureIsDisabled()
        //{
        //    // Arrange
        //    _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
        //        .Returns(Build(null));

        //    // Act
        //    var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext);

        //    // Assert
        //    Assert.That(isFeatureEnabled, Is.False);
        //}

        //private FeatureToggleConfiguration Build(string whitelistPattern)
        //{
        //    return new FeatureToggleConfiguration
        //    {
        //        Data = new FeatureToggleCollection
        //        {
        //            Features = new List<Domain.Models.FeatureToggles.FeatureToggle>
        //            {
        //                new Domain.Models.FeatureToggles.FeatureToggle
        //                {
        //                    Actions = new List<ControllerAction>
        //                    {
        //                        new ControllerAction
        //                        {
        //                            Controller = ControllerName,
        //                            Action = ActionName
        //                        }
        //                    },
        //                    Whitelist = new WhiteList {Emails = string.IsNullOrWhiteSpace(whitelistPattern) ? null : new List<string> {whitelistPattern}}
        //                }
        //            }
        //        }
        //    };
        //    var isFeatureEnabled = _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext);
        //}

        //[Test]
        //public void ThenShouldLogTrueResultIfFeatureIsEnabled()
        //{
        //    // Act
        //    _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext);

        //    // Assert
        //    _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{_membershipContext.UserId}' is '{true}'."), Times.Once);
        //}

        //[Test]
        //public void ThenShouldLogFalseResultIfFeatureIsDisabled()
        //{
        //    // Arrange
        //    _featureToggleServiceMock.Setup(f => f.GetDataFromTableStorage())
        //        .Returns(Build(null));

        //    // Act
        //    _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, _membershipContext);

        //    // Assert
        //    _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{_membershipContext.UserId}' is '{false}'."), Times.Once);
        //}

        //[Test]
        //public void ThenShouldLogNullUserExternalIdIfMembershipIsNull()
        //{
        //    // Act
        //    _featureToggleService.IsFeatureEnabled(ControllerName, ActionName, null);

        //    // Assert
        //    _logger.Verify(l => l.Info($"Is feature enabled check for controllerName '{ControllerName}', actionName '{ActionName}' and userId '{null}' is '{true}'."), Times.Once);
        //}
    }
}
