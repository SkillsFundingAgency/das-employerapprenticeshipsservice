using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Caching
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set(string key, object value, TimeSpan slidingExpiration);
        void Set(string key, object value, DateTimeOffset absoluteExpiration);
        Task<T> GetOrAdd<T>(string key, Func<string, Task<T>> getter, DateTimeOffset absoluteExpiration);
    }
}
