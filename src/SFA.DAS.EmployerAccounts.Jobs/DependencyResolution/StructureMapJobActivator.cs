using Microsoft.Azure.WebJobs.Host;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs.DependencyResolution
{
    //TODO Needs to be moved to a shared location
    public class StructureMapJobActivator : IJobActivator
    {
        private readonly IContainer _container;

        public StructureMapJobActivator(IContainer container)
        {
            _container = container;
        }

        public T CreateInstance<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}