using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.Caching
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

        public async Task<T> GetOrAdd<T>(string key, Func<string,Task<T>> getter, DateTimeOffset absoluteExpiration)
        {
            T result = default(T);

            if (!MemoryCache.Default.Contains(key))
            {
                result = await getter(key);
            }

            return (T) MemoryCache.Default.AddOrGetExisting(key, result, new CacheItemPolicy {AbsoluteExpiration = absoluteExpiration} );
        }
    }
}