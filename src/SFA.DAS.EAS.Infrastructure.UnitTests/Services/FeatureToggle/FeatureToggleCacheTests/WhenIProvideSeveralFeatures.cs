using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggle.FeatureToggleCacheTests
{
    [TestFixture]
    public class WhenIProvideSeveralFeatures
    {
        private FeatureToggleCollection BuildToggledFeatures(int requiredFeatures)
        {
            var featureConfig = FeatureToggleCollectionBuilder.Create();
            for (int i = 0; i < requiredFeatures; i++)
            {
                var feature = FeatureToggleBuilder.Create($"feature-{i}").WithControllerAction("ToggledController-{i}", "index");
                featureConfig.WithFeature(feature);
            }
            return featureConfig;
        }

        [Test]
        public void ThenFeatureShouldBeSubjectToAToggle()
        {
            // arrange
            var featureConfig = BuildToggledFeatures(3);
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(featureConfig);

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeatureToggle(featureConfig.Features[0].Actions[0].Controller);

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenActionShouldBeSubjectToAToggle()
        {
            // arrange
            var featureConfig = BuildToggledFeatures(3);
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(featureConfig);

            // act
            var controller = featureConfig.Features[0].Actions[0].Controller;
            var action = featureConfig.Features[0].Actions[0].Action;
            var isControllerSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle(controller, action, out _);

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }
    }
}