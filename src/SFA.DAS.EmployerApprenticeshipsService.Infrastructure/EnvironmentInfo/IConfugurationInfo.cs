using System;

namespace SFA.DAS.EAS.Infrastructure.EnvironmentInfo
{
    public interface IConfigurationInfo<T>
    {
        T GetConfiguration(string serviceName);
        T GetConfiguration(string serviceName, Action<string> action);
    }
}