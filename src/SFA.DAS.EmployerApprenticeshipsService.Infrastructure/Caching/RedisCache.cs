using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching;
using StackExchange.Redis;

namespace SFA.DAS.EAS.Infrastructure.Caching
{
    public class RedisCache : ICache
    {
        private readonly IDatabase _cache;

        public RedisCache()
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect(CloudConfigurationManager.GetSetting("RedisConnection"));
            _cache = connectionMultiplexer.GetDatabase();
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _cache.KeyExistsAsync(key);
        }

        public async Task<T> GetCustomValueAsync<T>(string key)
        {
            var redisValue = await _cache.StringGetAsync(key);

            return JsonConvert.DeserializeObject<T>(redisValue);
        }

        public async Task SetCustomValueAsync<T>(string key, T customType, int secondsInCache = 300)
        {
            if (!await _cache.KeyExistsAsync(key))
            {
                var _lock = new TaskSynchronizationScope();

                await _lock.RunAsync(async () =>
                {
                    if (!await _cache.KeyExistsAsync(key))
                    {
                        await _cache.StringSetAsync(key, JsonConvert.SerializeObject(customType), TimeSpan.FromSeconds(secondsInCache));
                    }
                });
            }
        }
    }
}