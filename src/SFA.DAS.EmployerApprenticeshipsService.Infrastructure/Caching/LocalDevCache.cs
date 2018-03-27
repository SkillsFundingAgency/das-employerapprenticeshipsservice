using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Microsoft.Azure;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Interfaces;
using StackExchange.Redis;

namespace SFA.DAS.EAS.Infrastructure.Caching
{
    /// <summary>
    ///     This class is only used during dev. It exists as a substitute for the Redis cache to avoid having a 
    ///     local dependency on Redis.
    ///     To keep a more faithful behaviour to using Redis the objects passed in will be serialised to json
    ///     as they are with our Redis wrapper.
    /// </summary>
    public class LocalDevCache : IDistributedCache
    {
        private static readonly Lazy<MemoryCache> DevCache = new Lazy<MemoryCache>(() => new MemoryCache("LocalDev"));

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(DevCache.Value.Contains(key));
        }

        public Task<T> GetCustomValueAsync<T>(string key)
        {
            var cachedValue = (string) DevCache.Value[key];

            return Task.FromResult(JsonConvert.DeserializeObject<T>(cachedValue));
        }

        public Task SetCustomValueAsync<T>(string key, T customType, TimeSpan cacheTime)
        {
            DevCache.Value.Add(key, JsonConvert.SerializeObject(customType), new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.Add(cacheTime)});
            return Task.CompletedTask;
        }

        public Task SetCustomValueAsync<T>(string key, T customType)
        {
            return SetCustomValueAsync(key, customType, Constants.DefaultCacheTime);
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter, TimeSpan maxCacheTime)
        {
            T result;

            if (DevCache.Value.Contains(key))
            {
                var existingValue = (string) DevCache.Value[key];
                result = JsonConvert.DeserializeObject<T>(existingValue);
            }
            else
            {
                result = await getter(key);
                await SetCustomValueAsync(key, result, maxCacheTime);
            }

            return result;
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter)
        {
            return GetOrAddAsync<T>(key, getter, Constants.DefaultCacheTime);
        }

        public Task RemoveFromCache(string key)
        {
            DevCache.Value.Remove(key);
            return Task.CompletedTask;
        }
    }
}