﻿using Newtonsoft.Json;
using SFA.DAS.Caches;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Helpers;
using SFA.DAS.EmployerFinance.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class CacheStorageService : ICacheStorageService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly EmployerFinanceConfiguration _config;

        public CacheStorageService(IDistributedCache distributedCache, EmployerFinanceConfiguration config)
        {
            _distributedCache = distributedCache;
            _config = config;
        }

        public async Task Save<T>(string key, T item, int expirationInMinutes)
        {
            var json = JsonConvert.SerializeObject(item);
            await _distributedCache.SetCustomValueAsync(key, json, TimeSpan.FromMinutes(_config.DefaultCacheExpirationInMinutes));
        }

        public bool TryGet(string key, out string value)
        {
            value = AsyncHelper.RunSync(() => Get<string>(key));
            return value != null;
        }

        private async Task<T> Get<T>(string key)
        {
            var json = string.Empty;
            if (await _distributedCache.ExistsAsync(key))
            {
                json = await _distributedCache.GetCustomValueAsync<string>(key);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task Delete(string key)
        {
            await _distributedCache.RemoveFromCache(key);
        }
    }
}
