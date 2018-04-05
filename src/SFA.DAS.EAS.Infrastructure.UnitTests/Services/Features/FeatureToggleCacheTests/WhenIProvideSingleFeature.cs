using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.Features.FeatureToggleCacheTests
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
            var fixtures = new ToggleFeatureTestFixtures()
                                .WithFeature(FeatureType.Transfer, $"{TestControllerName}.{TestActionName}");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeature(TestControllerName);

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenActionShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature(FeatureType.Transfer, $"{TestControllerName}.{TestActionName}");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToFeature(TestControllerName, TestActionName, out _);

            // assert
            Assert.IsTrue(isActionSubjectToToggle);
        }

        [Test]
        public void ThenFeatureToggleShouldBeCaseInsensitive()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature(FeatureType.Transfer, $"{TestControllerName}.{TestActionName}");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeature(TestControllerName.InvertCase());

            // assert
            Assert.IsTrue(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenActionToggleShouldBeCaseInsenstive()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature(FeatureType.Transfer, $"{TestControllerName}.{TestActionName}");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToFeature(TestControllerName.InvertCase(), TestActionName.InvertCase(), out _);

            // assert
            Assert.IsTrue(isActionSubjectToToggle);
        }

        [Test]
        public void ThenNoneToggeledControllerShouldNotBeSubjectToToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature(FeatureType.Transfer, $"{TestControllerName}.{TestActionName}");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isActionSubjectToToggle = ftc.IsControllerSubjectToFeature("NonToggledController");

            // assert
            Assert.IsFalse(isActionSubjectToToggle);
        }

        [Test]
        public void ThenNoneToggeledActionShouldNotBeSubjectToToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures()
                .WithFeature(FeatureType.Transfer, $"{TestControllerName}.{TestActionName}");

            var ftc = fixtures.CreateFixtureCache();

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToFeature("NonToggledController", "Index", out _);

            // assert
            Assert.IsFalse(isActionSubjectToToggle);
        }
    }
}