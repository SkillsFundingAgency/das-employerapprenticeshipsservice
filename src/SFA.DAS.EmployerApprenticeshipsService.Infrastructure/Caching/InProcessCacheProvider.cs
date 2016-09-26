using System;
using System.Runtime.Caching;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching
{
    public class InProcessCacheProvider : ICacheProvider
    {
        public T Get<T>(string key)
        {
            return (T)MemoryCache.Default.Get(key);
        }

        public void Set(string key, object value, TimeSpan slidingExpiration)
        {
            MemoryCache.Default.Set(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }

        public void Set(string key, object value, DateTimeOffset absoluteExpiration)
        {
            MemoryCache.Default.Set(key, value, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration });
        }
    }
}