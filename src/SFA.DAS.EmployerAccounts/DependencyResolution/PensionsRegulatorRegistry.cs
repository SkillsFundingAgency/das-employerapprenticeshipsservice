using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class PensionsRegulatorRegistry : Registry
{
    public PensionsRegulatorRegistry()
    {
        For<IPensionRegulatorService>().Use<PensionRegulatorService>();
    }
}