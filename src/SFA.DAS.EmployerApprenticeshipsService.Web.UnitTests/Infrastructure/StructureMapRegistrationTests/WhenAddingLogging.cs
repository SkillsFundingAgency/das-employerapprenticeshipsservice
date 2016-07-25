using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.DepedencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Infrastructure.StructureMapRegistrationTests
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
