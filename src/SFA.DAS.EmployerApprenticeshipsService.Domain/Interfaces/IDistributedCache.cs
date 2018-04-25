using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDistributedCache
    {
        Task<bool> ExistsAsync(string key);
        Task<T> GetCustomValueAsync<T>(string key);

        Task SetCustomValueAsync<T>(string key, T customType);
        Task SetCustomValueAsync<T>(string key, T customType, TimeSpan maxCacheTime);

        Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter, TimeSpan maxCacheTime);
        Task<T> GetOrAddAsync<T>(string key, Func<string, Task<T>> getter);

        Task RemoveFromCache(string key);
    }
}