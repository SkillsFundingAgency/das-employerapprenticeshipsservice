using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Interfaces;
using StackExchange.Redis;

namespace SFA.DAS.EAS.Infrastructure.Caching
{
    public class RedisCache : IDistributedCache
    {
        private readonly Lazy<IDatabase> _cache = new Lazy<IDatabase>(InitialiseRedis);

        public async Task<bool> ExistsAsync(string key)
        {
            return await _cache.Value.KeyExistsAsync(key);
        }

        public async Task<T> GetCustomValueAsync<T>(string key)
        {
            var redisValue = await _cache.Value.StringGetAsync(key);

            return JsonConvert.DeserializeObject<T>(redisValue);
        }

        public Task SetCustomValueAsync<T>(string key, T customType, TimeSpan cacheTime)
        {
            return _cache.Value.StringSetAsync(key, JsonConvert.SerializeObject(customType), cacheTime);
        }

        public Task SetCustomValueAsync<T>(string key, T customType)
        {
            return _cache.Value.StringSetAsync(key, JsonConvert.SerializeObject(customType), Constants.DefaultCacheTime);
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter, TimeSpan maxCacheTime)
        {
            T result;

            var existingValue = await _cache.Value.StringGetAsync(key);

            if (existingValue.IsNull)
            {
                result = await getter(key);
                await SetCustomValueAsync(key, result, maxCacheTime);
            }
            else
            {
                result = JsonConvert.DeserializeObject<T>(existingValue);
            }

            return result;
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter)
        {
            return GetOrAddAsync<T>(key, getter, Constants.DefaultCacheTime);
        }

        public Task RemoveFromCache(string key)
        {
            return _cache.Value.KeyDeleteAsync(key);
        }

        private static IDatabase InitialiseRedis()
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect(CloudConfigurationManager.GetSetting("RedisConnection"));
            var cache = connectionMultiplexer.GetDatabase();
            return cache;
        }
    }
}