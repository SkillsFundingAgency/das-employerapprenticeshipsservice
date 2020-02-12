using SFA.DAS.Encoding;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class EncodingRegistry : Registry
    {
        public EncodingRegistry()
        {
            For<IEncodingService>().Use<EncodingService>().Singleton();
        }
    }
}
