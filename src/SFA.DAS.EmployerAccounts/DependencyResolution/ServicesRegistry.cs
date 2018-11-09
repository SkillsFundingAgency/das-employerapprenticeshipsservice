using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class ServicesRegistry : Registry
    {
        public ServicesRegistry()
        {
            For<IAddressLookupService>().Use<AddressLookupService>();
            For<IRestClientFactory>().Use<RestClientFactory>();
            For<IRestServiceFactory>().Use<RestServiceFactory>();
        }
    }
}