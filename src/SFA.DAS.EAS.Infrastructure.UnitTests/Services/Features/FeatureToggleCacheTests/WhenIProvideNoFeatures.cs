using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.Features;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.Features.FeatureToggleCacheTests
{
    [TestFixture]
    public class WhenIProvideNoFeatures
    {
        [Test]
        public void ThenNoFeaturesShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures();
            var ftc = new FeatureCache(fixtures.Features, fixtures.ControllerMetaDataService);

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeature("foo");

            // assert
            Assert.IsFalse(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenNoActionShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures();
            var ftc = new FeatureCache(fixtures.Features, fixtures.ControllerMetaDataService);
            var operationContext = new AuthorizationContext { CurrentFeature = new Feature()};

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToFeature("Foo", "Bar", out _);

            // assert
            Assert.IsFalse(isActionSubjectToToggle);
        }
    }

    internal static class StringExtensions
    {
        public static string InvertCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            var newString = new char[s.Length];

            for (int i = 0; i < newString.Length; i++)
            {
                var c = s[i];
                if (char.IsLetter(c))
                {
                    c = char.IsLower(c) ? char.ToUpper(c) : char.ToLower(c);
                }

                newString[i] = c;
            }

            return new string(newString);
        }
    }

    public class ToggleFeatureTestFixtures
    {
        private readonly List<Feature> _features;

        public ToggleFeatureTestFixtures()
        {
            ControllerMetaDataServiceMock = new Mock<IControllerMetaDataService>();    
            _features = new List<Feature>();
        }

        public IControllerMetaDataService ControllerMetaDataService => ControllerMetaDataServiceMock.Object;
        public Mock<IControllerMetaDataService> ControllerMetaDataServiceMock { get; }

        public Feature[] Features => _features.ToArray();

        public ToggleFeatureTestFixtures WithFeature(FeatureType featureType, params string[] actions)
        {
            return WithWhitelistedFeature(featureType, null, actions);
        }

        public ToggleFeatureTestFixtures WithWhitelistedFeature(FeatureType featureType, string whitelistedEmail, params string[] actions)
        {
            var controllerActions = actions.Select(action =>
            {
                var parts = action.Split('.');

                return new ControllerAction(parts[0], parts[1]);
            }).ToArray();

            ControllerMetaDataServiceMock
                .Setup(svc => svc.GetControllerMethodsLinkedToAFeature(featureType))
                .Returns(controllerActions);

            AddFeature(featureType, whitelistedEmail);

            return this;
        }

        public ToggleFeatureTestFixtures BuildToggledFeatures(int requiredFeatures, params string[] actions)
        {
            for (int i = 0; i < requiredFeatures; i++)
            {
                WithWhitelistedFeature((FeatureType) i, $"email-{i}", actions);
            }

            return this;
        }

        public ToggleFeatureTestFixtures AddFeature(FeatureType featureType, string whitelistedEmail)
        {
            var newFeature = new Domain.Models.FeatureToggles.Feature
            {
                FeatureType = featureType,
                Enabled = true,
                Name = featureType.ToString()
            };

            _features.Add(newFeature);

            if (whitelistedEmail != null)
            {
                newFeature.Whitelist = new[] {whitelistedEmail};
            }

            return this;
        }

        public ToggleFeatureTestFixtures AddFeature(FeatureType featureType)
        {
            AddFeature(featureType, null);
            return this;
        }

        public FeatureCache CreateFixtureCache()
        {
            return new FeatureCache(Features, ControllerMetaDataService);
        }
    }
}
