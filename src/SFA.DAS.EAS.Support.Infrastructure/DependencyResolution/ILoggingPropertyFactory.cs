using System.Collections.Generic;

namespace SFA.DAS.EAS.Support.Infrastructure.DependencyResolution
{
    public interface ILoggingPropertyFactory
    {
        IDictionary<string, object> GetProperties();
    }
}