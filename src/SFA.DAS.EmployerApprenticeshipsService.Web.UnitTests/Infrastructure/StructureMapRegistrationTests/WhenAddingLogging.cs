using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Web.UnitTests.Infrastructure.StructureMapRegistrationTests
{
    public class WhenAddingLogging
    {
        private Container _container;

        [SetUp]
        public void Arrange()
        {
            _container = new Container(
                c =>
                    {
                        c.AddRegistry<TestRegistry>();
                        c.Policies.Add<LoggingPolicy>();
                    }
                );
        }

        [Test]
        public void ThenTheLoggerCanBeResolvedForThatClass()
        {
            //Act
            var actual = _container.GetInstance<TestClass>();

            //Assert
            Assert.IsNotNull(actual.Logger);
            
        }

        [Test]
        public void ThenTheLoggerHasBeenNamedWithTheFullNameOfThatClass()
        {
            //Act
            var actual = _container.GetInstance<TestClass>();
            //Assert
            Assert.AreEqual(typeof(TestClass).FullName, actual.Logger.Name);
        }

        public interface ITestClass
        {

        }

        public class TestClass : ITestClass
        {
            public readonly NLog.ILogger Logger;

            public TestClass(NLog.ILogger logger)
            {
                Logger = logger;
            }
        }
        public class TestRegistry : Registry
        {
            public TestRegistry()
            {
                For<ITestClass>().Use<TestClass>();
            }
        }
    }
}
