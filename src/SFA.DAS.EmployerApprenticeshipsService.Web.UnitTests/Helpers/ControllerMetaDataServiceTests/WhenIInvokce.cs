using System.Web.Mvc;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.Features;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.UnitTests.Helpers.ControllerMetaDataServiceTests
{

    [TestFixture]
    public class WhenIInvokceGetControllerMethodsLinkedToAFeature
    {
        [Test]
        public void ItShouldNotThrowException()
        {
            var controllerMetaDataService = new ControllerMetaDataService();

            var foundControllers = controllerMetaDataService.GetControllerMethodsLinkedToAFeature(FeatureType.Test1);

            Assert.Pass("Should not expect an exception");
        }

        [Test]
        public void ItShouldNotReturnNull()
        {
            var controllerMetaDataService = new ControllerMetaDataService();

            var foundControllers = controllerMetaDataService.GetControllerMethodsLinkedToAFeature(FeatureType.Test1);

            Assert.IsNotNull(foundControllers);
        }

        [TestCase(FeatureType.Test1, "Test1.*", "Test2.Method1", "Test2.Method2", "Test3.Method1", "Test5.*")]
        [TestCase(FeatureType.Test2, "Test5.Method1")]
        [TestCase(FeatureType.NotSpecified)]
        public void ItShouldCorrectlyLinkControllerActionsToFeatures(FeatureType featureType, params string[] expectedControllerActionNames)
        {
            // arrange
            var controllerMetaDataService = new ControllerMetaDataService(this.GetType().Assembly);

            // Act
            var actualControllerActions = controllerMetaDataService.GetControllerMethodsLinkedToAFeature(featureType);

            var expectedControllerActions = expectedControllerActionNames.Select(name => new ControllerAction(name)).ToArray();
            var expectedButNotActual = expectedControllerActions.Except(actualControllerActions).ToArray();
            var actualButNotExpected = actualControllerActions.Except(expectedControllerActions).ToArray();

            // Assert
            Assert.IsTrue(expectedButNotActual.Length == 0, $"Expected controllers actions were not linked to feature {featureType}: {string.Join(",", expectedControllerActions.Select(ca => ca.QualifiedName))}");
            Assert.IsTrue(actualButNotExpected.Length == 0, $"Unexpected controller actions were linked to feature {featureType}: {string.Join(",", actualButNotExpected.Select(ca => ca.QualifiedName))}");
        }
    }

    [OperationFilter(RequiresAccessToFeatureType = FeatureType.Test1)]
    internal class Test1Controller : Controller
    {
        // The operaton filter methods here should be irrelevant
        public void Method1()
        {
        }

        [OperationFilter(RequiresAccessToFeatureType = FeatureType.Test1)]
        public void Method2()
        {
        }
    }
    internal class Test2Controller : Controller
    {
        // Some methods are linked to features, some are not
        [OperationFilter(RequiresAccessToFeatureType = FeatureType.Test1)]
        public void Method1()
        {
        }

        [OperationFilter(RequiresAccessToFeatureType = FeatureType.Test1)]
        public void Method2()
        {
        }

        public void Method3()
        {
        }
    }

    internal class Test3Controller : Controller
    {
        [OperationFilter(RequiresAccessToFeatureType = FeatureType.Test1)]
        public void Method1()
        {
        }
    }

    internal class Test4Controller : Controller
    {
        // Method linked to not specified should be ignored
        [OperationFilter(RequiresAccessToFeatureType = FeatureType.NotSpecified)]
        public void Method1()
        {
        }
    }

    [OperationFilter(RequiresAccessToFeatureType = FeatureType.Test1)]
    internal class Test5Controller : Controller
    {
        // Method should pick up class feature as well as feature specified here
        [OperationFilter(RequiresAccessToFeatureType = FeatureType.Test2)]
        public void Method1()
        {
        }
    }
}
