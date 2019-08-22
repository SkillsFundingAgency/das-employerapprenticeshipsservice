using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class PensionsRegulatorRegistry : Registry
    {
        public PensionsRegulatorRegistry()
        {
            For<IPensionRegulatorService>().Use<PensionRegulatorService>();
        }
    }
}