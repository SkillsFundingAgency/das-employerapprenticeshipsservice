using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using StructureMap;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    public class IntegrationTestDependencyResolver : IDependencyResolver
    {

        private readonly HashSet<Type> _createdServices = new HashSet<Type>();

        public IntegrationTestDependencyResolver(IContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void ClearCreationLog()
        {
            _createdServices.Clear();
        }

        public IContainer Container { get; set; }

        public IEnumerable<Type> CreatedServiceTypes => _createdServices;

        public bool WasServiceCreated(Type serviceType)
        {
            return _createdServices.Contains(serviceType);
        }

        public void Dispose()
        {
            Container.Dispose();    
        }

        public object GetService(Type serviceType)
        {
            RegisterServiceCreation(serviceType);
            return DoGetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Container.GetAllInstances(serviceType).OfType<object>();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        protected object DoGetInstance(Type serviceType)
        {
            return serviceType.IsAbstract || serviceType.IsInterface
                ? Container.TryGetInstance(serviceType)
                : Container.GetInstance(serviceType);
        }

        private void RegisterServiceCreation(Type serviceType)
        {
            if (!_createdServices.Contains(serviceType))
            {
                _createdServices.Add(serviceType);
            }
        }
    }
}