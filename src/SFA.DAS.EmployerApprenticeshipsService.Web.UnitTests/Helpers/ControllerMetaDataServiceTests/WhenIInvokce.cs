using System.Web.Mvc;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Services.FeatureToggle;
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

            var foundControllers = controllerMetaDataService.GetControllerMethodsLinkedToAFeature(Feature.Test1);

            Assert.Pass("Should not expect an exception");
        }

        [Test]
        public void ItShouldNotReturnNull()
        {
            var controllerMetaDataService = new ControllerMetaDataService();

            var foundControllers = controllerMetaDataService.GetControllerMethodsLinkedToAFeature(Feature.Test1);

            Assert.IsNotNull(foundControllers);
        }

        [TestCase(Feature.Test1, "Test1.*", "Test2.Method1", "Test2.Method2", "Test3.Method1", "Test5.*")]
        [TestCase(Feature.Test2, "Test3.Method1", "Test5.Method1")]
        [TestCase(Feature.NotSpecified)]
        public void ItShouldCorrectlyLinkControllerActionsToFeatures(Feature feature, params string[] expectedControllerActionNames)
        {
            // arrange
            var controllerMetaDataService = new ControllerMetaDataService();

            // Act
            var actualControllerActions = controllerMetaDataService.GetControllerMethodsLinkedToAFeature(feature);

            var expectedControllerActions = expectedControllerActionNames.Select(name => new ControllerAction(name)).ToArray();
            var expectedButNotActual = expectedControllerActions.Except(actualControllerActions).ToArray();
            var actualButNotExpected = actualControllerActions.Except(expectedControllerActions).ToArray();

            // Assert
            Assert.AreEqual(0, expectedButNotActual.Length, $"Expected items not found:{string.Join(",", expectedControllerActions.Select(ca => ca.QualifiedName))}");
            Assert.AreEqual(0, actualButNotExpected.Length, $"Unexpected items found:{string.Join(",", actualButNotExpected.Select(ca => ca.QualifiedName))}");
        }
    }

    [OperationFilter(RequiresAccessToFeature = Feature.Test1)]
    internal class Test1Controller : Controller
    {
        // The operaton filter methods here should be irrelevant
        public void Method1()
        {
        }

        [OperationFilter(RequiresAccessToFeature = Feature.Test1)]
        public void Method2()
        {
        }
    }
    internal class Test2Controller : Controller
    {
        // Some methods are linked to features, some are not
        [OperationFilter(RequiresAccessToFeature = Feature.Test1)]
        public void Method1()
        {
        }

        [OperationFilter(RequiresAccessToFeature = Feature.Test1)]
        public void Method2()
        {
        }

        public void Method3()
        {
        }
    }

    internal class Test3Controller : Controller
    {
        // Method linked to multiple features
        [OperationFilter(RequiresAccessToFeature = Feature.Test1)]
        [OperationFilter(RequiresAccessToFeature = Feature.Test2)]
        public void Method1()
        {
        }
    }

    internal class Test4Controller : Controller
    {
        // Method linked to not specified should be ignored
        [OperationFilter(RequiresAccessToFeature = Feature.NotSpecified)]
        public void Method1()
        {
        }
    }

    [OperationFilter(RequiresAccessToFeature = Feature.Test1)]
    internal class Test5Controller : Controller
    {
        // Method should pick up class feature as well as feature specified here
        [OperationFilter(RequiresAccessToFeature = Feature.Test2)]
        public void Method1()
        {
        }
    }
}
