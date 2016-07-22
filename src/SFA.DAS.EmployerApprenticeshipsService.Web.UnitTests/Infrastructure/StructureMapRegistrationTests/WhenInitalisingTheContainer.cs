using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Web.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Infrastructure.StructureMapRegistrationTests
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
                        c.Policies.Add<ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>>();
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

        public class TestRegistry : Registry
        {
            public TestRegistry()
            {
                For<ITestClass>().Use<TestClass>();
            }
        }

        public interface ITestClass
        {
            
        }

        public class TestClass : ITestClass
        {
            public readonly EmployerApprenticeshipsServiceConfiguration Configuration;

            public TestClass(EmployerApprenticeshipsServiceConfiguration configuration)
            {
                Configuration = configuration;
            }
        }
    }
}
