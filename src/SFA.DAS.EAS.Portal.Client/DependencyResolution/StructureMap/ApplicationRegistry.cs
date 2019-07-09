using SFA.DAS.EAS.Portal.Client.Application.Queries;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class ApplicationRegistry : Registry
    {
        public ApplicationRegistry()
        {
            For<IGetAccountQuery>().Use<GetAccountQuery>().Singleton();
        }
    }
}