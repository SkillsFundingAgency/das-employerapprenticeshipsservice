using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.FeatureToggle.FeatureToggleCacheTests
{
    [TestFixture]
    public class WhenIProvideNoFeatures
    {
        [Test]
        public void ThenNoFeaturesShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures();
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(fixtures.FeatureToggleCollection, fixtures.ControllerMetaDataService);

            // act
            var isControllerSubjectToToggle = ftc.IsControllerSubjectToFeatureToggle("foo");

            // assert
            Assert.IsFalse(isControllerSubjectToToggle);
        }

        [Test]
        public void ThenNoActionShouldBeSubjectToAToggle()
        {
            // arrange
            var fixtures = new ToggleFeatureTestFixtures();
            var ftc = new Infrastructure.Services.FeatureToggle.FeatureToggleCache(fixtures.FeatureToggleCollection, fixtures.ControllerMetaDataService);

            // act
            var isActionSubjectToToggle = ftc.TryGetControllerActionSubjectToToggle("foo", "action", out _);

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
        public ToggleFeatureTestFixtures()
        {
            ControllerMetaDataServiceMock = new Mock<IControllerMetaDataService>();    
            FeatureToggleCollection = new FeatureToggleCollection();
        }

        public IControllerMetaDataService ControllerMetaDataService => ControllerMetaDataServiceMock.Object;
        public Mock<IControllerMetaDataService> ControllerMetaDataServiceMock { get; }

        public FeatureToggleCollection FeatureToggleCollection { get; set; }

        public ToggleFeatureTestFixtures WithFeature(Feature feature, params string[] actions)
        {
            return WithWhiteListedFeature(feature, null, actions);
        }


        public ToggleFeatureTestFixtures WithWhiteListedFeature(Feature feature, string whitelistedEmail, params string[] actions)
        {
            var controllerActions = actions.Select(action =>
            {
                var parts = action.Split('.');

                return new ControllerAction(parts[0], parts[1]);
            }).ToArray();

            ControllerMetaDataServiceMock
                .Setup(svc => svc.GetControllerMethodsLinkedToAFeature(feature))
                .Returns(controllerActions);

            AddFeature(feature, whitelistedEmail);

            return this;
        }

        public ToggleFeatureTestFixtures BuildToggledFeatures(int requiredFeatures, params string[] actions)
        {
            for (int i = 0; i < requiredFeatures; i++)
            {
                WithWhiteListedFeature((Feature) i, $"email-{i}", actions);
            }

            return this;
        }

        public ToggleFeatureTestFixtures AddFeature(Feature feature, string whitelistedEmail)
        {
            var newFeatureToggle = new Domain.Models.FeatureToggles.FeatureToggle
            {
                Feature = feature,
                Enabled = true,
                Name = feature.ToString()
            };

            FeatureToggleCollection.Features.Add(newFeatureToggle);

            if (whitelistedEmail != null)
            {
                newFeatureToggle.Whitelist.Emails.Add(whitelistedEmail);
            }

            return this;
        }

        public ToggleFeatureTestFixtures AddFeature(Feature feature)
        {
            AddFeature(feature, null);
            return this;
        }

        public FeatureToggleCache CreateFixtureCache()
        {
            return new Infrastructure.Services.FeatureToggle.FeatureToggleCache(FeatureToggleCollection, ControllerMetaDataService);
        }
    }
}
