using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Services.Features;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.Features
{
    [TestFixture]
    public class FeatureServiceTests
    {
        [Test]
        public void Constructor_ValidCall_ShouldSucceed()
        {
            var featureService = new FeatureServiceTestFixtures().CreateFeatureService();
            Assert.Pass("Shouldn't get exception");
        }

        [Test]
        public Task GetWhitelistForOperationAsync_ForUnfeaturedEndPoint_ShouldReturnNullForWhiteList()
        {
            return CheckUnfeaturedOperation((fixtures, whitelist) => Assert.IsNull(whitelist));
        }

        private async Task CheckUnfeaturedOperation(Action<FeatureServiceTestFixtures, string[]> check)
        {
            var fixtures = new FeatureServiceTestFixtures()
                                .WithNoControllersLinkedToFeatures();
           
            var featureService = fixtures.CreateFeatureService();
            var operation = fixtures.CreateOperationaContext("unfeaturedController.index");


            var result = await featureService.GetWhitelistForOperationAsync(operation);

            check?.Invoke(fixtures, result);
        }
    }

    internal class FeatureServiceTestFixtures
    {
        public FeatureServiceTestFixtures()
        {
            CacheProviderMock = new Mock<ICacheProvider>();    
            LoggerMock = new Mock<ILog>();
            FeatureToggleCacheFactoryMock = new Mock<IFeatureCacheFactory>();
            FeatureToggleCacheMock = new Mock<IFeatureCache>();
        }

        public Mock<ICacheProvider> CacheProviderMock { get; }
        public ICacheProvider CacheProvider => CacheProviderMock.Object;

        public Mock<ILog> LoggerMock { get; }
        public ILog Logger => LoggerMock.Object;

        public Mock<IFeatureCacheFactory> FeatureToggleCacheFactoryMock { get; }
        public IFeatureCacheFactory FeatureCacheFactory => FeatureToggleCacheFactoryMock.Object;

        public Mock<IFeatureCache> FeatureToggleCacheMock { get; set; }
        public IFeatureCache FeatureCache => FeatureToggleCacheMock.Object;

        public FeatureService CreateFeatureService()
        {
            return new FeatureService(CacheProvider, Logger, FeatureCacheFactory);
        }

        public FeatureServiceTestFixtures WithNoControllersLinkedToFeatures()
        {
            ControllerActionCacheItem result = null;
            FeatureToggleCacheMock
                .Setup(ft => ft.TryGetControllerActionSubjectToFeature(It.IsAny<OperationContext>(), out result))
                .Returns(false);

            FeatureToggleCacheMock
                .Setup(ft => ft.IsControllerSubjectToFeature(It.IsAny<string>()))
                .Returns(false);

            CacheProviderMock
                .Setup(cp => cp.Get<IFeatureCache>(It.IsAny<string>()))
                .Returns(FeatureCache);

            return this;
        }
        
        public OperationContext CreateOperationaContext(string qualifiedControllerAction)
        {
            var ca = new ControllerAction(qualifiedControllerAction);

            return new OperationContext
            {
                Controller = ca.Controller,
                Action = ca.Action
            };
        }
    }
}
