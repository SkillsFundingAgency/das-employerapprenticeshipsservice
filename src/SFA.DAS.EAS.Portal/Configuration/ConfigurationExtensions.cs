using System;
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

        //todo: don't need section argument, but then clashes with core's. rename?
        // also, add IConfigurationSection returning method, and delegate above to below, throw in all if not found, exceptions for unable to find key and unable to deserialize
        public static TConfiguration GetSection<TConfiguration>(this IConfiguration configuration, string section, params string[] subSectionPaths)
        {
            var key = string.Join(":", Enumerable.Repeat(section, 1).Concat(subSectionPaths));

            var config = configuration.GetSection(key).Get<TConfiguration>();
            if (config == null)
                throw new Exception($"Unable to find configuration with key {key}");

            return config;
        }
    }
}