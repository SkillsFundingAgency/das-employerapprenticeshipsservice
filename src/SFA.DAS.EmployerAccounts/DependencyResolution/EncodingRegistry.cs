using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class EncodingRegistry : Registry
{
    public EncodingRegistry()
    {
        For<IEncodingService>().Use<EncodingService>().Singleton();
    }
}