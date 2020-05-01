using System;
using System.Threading.Tasks;
using SFA.DAS.Caches;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class CacheStorageService : ICacheStorageService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheStorageService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SaveToCache<T>(string key, T item, int expirationInHours)
        {
            var json = JsonConvert.SerializeObject(item);

            //await _distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationInHours)
            //});
            await _distributedCache.SetCustomValueAsync(key, json, new TimeSpan(1, 0, 0));
        }

        public async Task<T> RetrieveFromCache<T>(string key)
        {
            //var json = await _distributedCache.GetStringAsync(key);
            var json = string.Empty;
            if (await _distributedCache.ExistsAsync(key))
            {
                json = await _distributedCache.GetCustomValueAsync<string>(key);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task DeleteFromCache(string key)
        {
            //await _distributedCache.RemoveAsync(key);
            await _distributedCache.RemoveFromCache(key);
        }
    }
}