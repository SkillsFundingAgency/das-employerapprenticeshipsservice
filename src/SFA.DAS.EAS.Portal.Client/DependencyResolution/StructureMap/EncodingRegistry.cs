using SFA.DAS.Encoding;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class EncodingRegistry : Registry
    {
        public EncodingRegistry()
        {
            For<IEncodingService>().Use<EncodingService>().Singleton();
        }
    }
}