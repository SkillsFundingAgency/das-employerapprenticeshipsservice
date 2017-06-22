using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.NLog.Logger;
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

        public interface ITestClass
        {

        }

        public class TestClass : ITestClass
        {
            public readonly ILog Logger;

            public TestClass(ILog logger)
            {
                Logger = logger;
            }
        }
        public class TestRegistry : Registry
        {
            public TestRegistry()
            {
                For<ITestClass>().Use<TestClass>();
                For<ILog>().Use(() => new NLogLogger(null, null, null));
            }
        }
    }
}
