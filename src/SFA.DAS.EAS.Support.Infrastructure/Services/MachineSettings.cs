using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SFA.DAS.EAS.Support.Core.Services;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    [ExcludeFromCodeCoverage]
    public sealed class MachineSettings : IProvideSettings
    {
        public string GetSetting(string settingKey)
        {
            return Environment.GetEnvironmentVariable($"DAS_{settingKey.ToUpper(CultureInfo.InvariantCulture)}",
                EnvironmentVariableTarget.User);
        }

        public string GetNullableSetting(string settingKey)
        {
            return GetSetting(settingKey);
        }
    }
}