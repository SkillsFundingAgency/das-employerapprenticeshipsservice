using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace SFA.DAS.Caches
{
    public class RedisCache : IDistributedCache
    {
        private static string _redisConnection;
        private readonly Lazy<IDatabase> _cache = new Lazy<IDatabase>(InitialiseRedis);

        public RedisCache(string redisConnection)
        {
            _redisConnection = redisConnection;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return _cache.Value.KeyExistsAsync(key);
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter)
        {
            return GetOrAddAsync(key, getter, Constants.DefaultCacheTime);
        }

        public async Task<T> GetCustomValueAsync<T>(string key)
        {
            var cachedValue = await _cache.Value.StringGetAsync(key).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(cachedValue);
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter, TimeSpan maxCacheTime)
        {
            T result;

            var existingValue = await _cache.Value.StringGetAsync(key).ConfigureAwait(false);

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

        public Task RemoveFromCache(string key)
        {
            return _cache.Value.KeyDeleteAsync(key);
        }

        public Task SetCustomValueAsync<T>(string key, T customType)
        {
            return _cache.Value.StringSetAsync(key, JsonConvert.SerializeObject(customType),
                Constants.DefaultCacheTime);
        }

        public Task SetCustomValueAsync<T>(string key, T customType, TimeSpan cacheTime)
        {
            return _cache.Value.StringSetAsync(key, JsonConvert.SerializeObject(customType), cacheTime);
        }

        private static IDatabase InitialiseRedis()
        {
            var connectionMultiplexer =
                ConnectionMultiplexer.Connect(
                    (string.IsNullOrWhiteSpace(_redisConnection) ? null : _redisConnection) ??
                    CloudConfigurationManager.GetSetting("RedisConnection"));
            var cache = connectionMultiplexer.GetDatabase();

            return cache;
        }
    }
}