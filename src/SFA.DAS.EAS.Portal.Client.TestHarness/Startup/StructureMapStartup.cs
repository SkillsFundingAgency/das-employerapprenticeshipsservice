using Microsoft.Extensions.Hosting;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.TestHarness.Startup
{
    public static class StructureMapStartup
    {
        public static IHostBuilder UseStructureMap(this IHostBuilder builder)
        {
            return builder.UseServiceProviderFactory(new StructureMapServiceProviderFactory(null));
        }
    }
}