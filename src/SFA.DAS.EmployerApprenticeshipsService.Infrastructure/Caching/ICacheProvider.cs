using System;

namespace SFA.DAS.EAS.Infrastructure.Caching
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set(string key, object value, TimeSpan slidingExpiration);
        void Set(string key, object value, DateTimeOffset absoluteExpiration);
    }
}
