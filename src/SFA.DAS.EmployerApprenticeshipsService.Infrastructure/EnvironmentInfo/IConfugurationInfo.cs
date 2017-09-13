using System;

namespace SFA.DAS.EAS.Infrastructure.EnvironmentInfo
{
    public interface IConfugurationInfo<T>
    {
        T GetConfiguration(string serviceName, Action<string> action);
    }
}