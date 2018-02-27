using NUnit.Framework;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggle.FeatureToggleCacheTests
{
    [TestFixture]
    public class WhenIProvideSingleFeature
    {
        const string TestControllerName = "foo";
        const string TestActionName = "index";

        [Test]
        public void ThenFeatureShouldBeSubjectToAToggle()
        {
            // arrange
            var feature = FeatureToggleBuilder.Create("transfers").WithControllerAction(TestControllerName, "index");
            var featureConfig = FeatureToggleCollectionBuilder.Create().WithFeature(feature);
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(featureConfig);

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeatureToggle(TestControllerName);

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenActionShouldBeSubjectToAToggle()
        {
            // arrange
            var feature = FeatureToggleBuilder.Create("transfers").WithControllerAction(TestControllerName, TestActionName);
            var featureConfig = FeatureToggleCollectionBuilder.Create().WithFeature(feature);
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(featureConfig);

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle(TestControllerName, TestActionName, out _);

            // assert
            Assert.IsTrue(isActionSubjectToToggle);
        }

        [Test]
        public void ThenFeatureToggleShouldBeCaseInsensitive()
        {
            // arrange
            var feature = FeatureToggleBuilder.Create("transfers").WithControllerAction(TestControllerName, "index");
            var featureConfig = FeatureToggleCollectionBuilder.Create().WithFeature(feature);
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(featureConfig);

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeatureToggle(TestControllerName.InvertCase());

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenActionToggleShouldBeCaseInsenstive()
        {
            // arrange
            var feature = FeatureToggleBuilder.Create("transfers").WithControllerAction(TestControllerName, TestActionName);
            var featureConfig = FeatureToggleCollectionBuilder.Create().WithFeature(feature);
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(featureConfig);

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle(TestControllerName.InvertCase(), TestActionName.InvertCase(), out _);

            // assert
            Assert.IsTrue(isActionSubjectToToggle);
        }

        [Test]
        public void ThenNoneToggeledControllerShouldNotBeSubjectToToggle()
        {
            // arrange
            var feature = FeatureToggleBuilder.Create("transfers").WithControllerAction(TestControllerName, TestActionName);
            var featureConfig = FeatureToggleCollectionBuilder.Create().WithFeature(feature);
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(featureConfig);

            // act
            var isActionSubjectToToggle = ftc.IsControllerSubjectToFeatureToggle("NonToggledController");

            // assert
            Assert.IsFalse(isActionSubjectToToggle);
        }

        [Test]
        public void ThenNoneToggeledActionShouldNotBeSubjectToToggle()
        {
            // arrange
            var feature = FeatureToggleBuilder.Create("transfers").WithControllerAction(TestControllerName, TestActionName);
            var featureConfig = FeatureToggleCollectionBuilder.Create().WithFeature(feature);
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(featureConfig);

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle("NonToggledController", "Index", out _);

            // assert
            Assert.IsFalse(isActionSubjectToToggle);
        }
    }
}