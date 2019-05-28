using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.EAS.Portal.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationSection GetPortalSection(this IConfiguration configuration, params string[] subSectionPaths)
        {
            var key = string.Join(":", 
                Enumerable.Repeat(ConfigurationKeys.EmployerApprenticeshipsServicePortal, 1)
                    .Concat(subSectionPaths));
            return configuration.GetSection(key);
        }

        public static TConfiguration GetPortalSection<TConfiguration>(this IConfiguration configuration, params string[] subSectionPaths)
        {
            return configuration.GetPortalSection(subSectionPaths).Get<TConfiguration>();
        }

        public static TConfiguration GetSection<TConfiguration>(this IConfiguration configuration, string key,   params string[] subSectionPaths)
        {
            var temp = string.Join(":", Enumerable.Repeat(key, 1).Concat(subSectionPaths));
            return configuration.GetSection(temp).Get<TConfiguration>();
        }
    }
}