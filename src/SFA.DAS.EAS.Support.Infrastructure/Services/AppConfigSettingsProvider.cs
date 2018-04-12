using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure;
using SFA.DAS.EAS.Support.Core.Services;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    [ExcludeFromCodeCoverage]
    public class AppConfigSettingsProvider : IProvideSettings
    {
        private readonly IProvideSettings _baseSettings;

        public AppConfigSettingsProvider(IProvideSettings baseSettings)
        {
            _baseSettings = baseSettings;
        }

        public string GetSetting(string settingKey)
        {
            var setting = GetNullableSetting(settingKey);

            if (string.IsNullOrEmpty(setting))
                throw new ConfigurationErrorsException($"Setting with key {settingKey} is missing");

            return setting;
        }

        public string GetNullableSetting(string settingKey)
        {
            var setting = CloudConfigurationManager.GetSetting(settingKey);

            if (string.IsNullOrWhiteSpace(setting))
                setting = TryBaseSettingsProvider(settingKey);

            return setting;
        }

        private string TryBaseSettingsProvider(string settingKey)
        {
            return _baseSettings.GetSetting(settingKey);
        }
    }
}