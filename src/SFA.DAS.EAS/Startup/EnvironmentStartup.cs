using System;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Configuration;

namespace SFA.DAS.EAS.Startup
{
    public static class EnvironmentStartup
    {
        public static IHostBuilder UseDasEnvironment(this IHostBuilder hostBuilder)
        {
            var environmentName = Environment.GetEnvironmentVariable(EnvironmentVariableName.EnvironmentName);
            var mappedEnvironmentName = DasEnvironmentName.Map[environmentName];

            return hostBuilder.UseEnvironment(mappedEnvironmentName);
        }
    }
}