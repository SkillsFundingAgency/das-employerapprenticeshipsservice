using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RenameEmployerAccount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Web.UnitTests.Infrastructure.StructureMapRegistrationTests
{
    public class WhenAddingValidators
    {
        private const string ServiceNamespace = "SFA.DAS";
        private Container _container;

        [SetUp]
        public void Arrange()
        {
            _container = new Container(
                c =>
                {
                    c.AddRegistry<WhenAddingLogging.TestRegistry>();
                    c.Scan(scan => {
                        scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                        scan.RegisterConcreteTypesAgainstTheFirstInterface();
                        scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
                    }); 
                }
            );
        }

        [Test]
        public void ThenTheCorrectContreteTypeIsReturned()
        {
            //Act
            var actual = _container.GetInstance<IValidator<RenameEmployerAccountCommand>>();
            
            //Assert
            Assert.IsInstanceOf<RenameEmployerAccountCommandValidator>(actual);
        }

        [Test]
        public void ThenTheConcreteTypeIsSingleton()
        {
            //Act
            var actual = _container.GetInstance<IValidator<RenameEmployerAccountCommand>>();
            var another = _container.GetInstance<IValidator<RenameEmployerAccountCommand>>();

            //Assert
            Assert.AreSame(actual, another);          
        }

        [Test]
        public void ThenTheTypeIsRegisteredAsSingleton()
        {
            //Act
            var instance = _container.Model.AllInstances.First(x => x.PluginType == typeof(IValidator<RenameEmployerAccountCommand>));

            //Assert
            Assert.AreEqual("Singleton", instance.Lifecycle.Description);
        }

    }
}
