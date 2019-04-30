using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.EAS.Portal.Configuration
{
    public static class ConfigurationExtensions
    {
        public static TConfiguration GetPortalSection<TConfiguration>(this IConfiguration configuration, params string[] subSectionPaths)
        {
            var key = string.Join(":", Enumerable.Repeat(ConfigurationKeys.EmployerApprenticeshipsServicePortal, 1).Concat(subSectionPaths));
            var configurationSection = configuration.GetSection(key);
            var value = configurationSection.Get<TConfiguration>();

            return value;
        }
    }
}