using SFA.DAS.EAS.Support.Core.Services;
using StructureMap;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.EAS.Support.Web.DependencyResolution
{
    [ExcludeFromCodeCoverage]
    public class CoreRegistry : Registry
    {
        public CoreRegistry()
        {
            For<IPayeSchemeObfuscator>().Use<PayeSchemeObfuscator>();
        }
    }
}