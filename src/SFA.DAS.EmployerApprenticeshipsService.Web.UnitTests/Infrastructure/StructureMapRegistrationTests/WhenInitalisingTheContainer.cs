using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Web.UnitTests.Infrastructure.StructureMapRegistrationTests
{
    public class WhenInitalisingTheContainer
    {
        private Container _container;

        [SetUp]
        public void Arrange()
        {
            _container = new Container(
                c =>
                    {
                        c.AddRegistry<TestRegistry>();
                        c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                    }
                );
        }

        [Test]
        public void ThenTheConfigurationPolicyIsCorrectlyInitalised()
        {
            //Act
            var item = _container.GetInstance<TestClass>();

            //Assert
            Assert.IsNotNull(item.Configuration);
        }

        [Test]
        public void ThenANonRegisteredClassWithADefaultConstructorDoesNotError()
        {
            //Act
            var item = _container.GetInstance<TestController>();

            //Assert
            Assert.IsNotNull(item);
        }

        
        internal class TestRegistry : Registry
        {
            public TestRegistry()
            {
                For<ITestClass>().Use<TestClass>();
                For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            }
        }


        internal interface ITestClass
        {
            
        }

        internal class TestClass : ITestClass
        {
            public readonly IConfiguration Configuration;

            public TestClass(IConfiguration configuration)
            {
                Configuration = configuration;
            }
        }

        internal class TestController
        {
            
            public TestController()
            {
            }

            public TestController(ITestClass something)
            {
                
            }
        }
    }
}
