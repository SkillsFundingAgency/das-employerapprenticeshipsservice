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
            var operationContext = new OperationContext { Controller = TestControllerName, Action = TestActionName };

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToFeature(operationContext, out _);

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
            var operationContext = new OperationContext { Controller = TestControllerName.InvertCase(), Action = TestActionName.InvertCase() };

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToFeature(operationContext, out _);

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
            var operationContext = new OperationContext { Controller = "NonToggledController", Action = "Index" };

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToFeature(operationContext, out _);

            // assert
            Assert.IsFalse(isActionSubjectToToggle);
        }
    }
}