using System;
using System.Threading.Tasks;

namespace Lychee.Extension.Cache.Abstractions
{
    /// <summary>
    /// 缓存
    /// </summary>
    public interface ICache
    {
        bool Exists(string key);

        Task<bool> ExistsAsync(string key);

        T Get<T>(string key, Func<T> func, TimeSpan? expiration = null);

        Task<T> GetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null);

        bool TryAdd<T>(string key, T Value, TimeSpan? expiration = null);

        Task<bool> TryAddAsync<T>(string key, T value, TimeSpan? expiration = null);

        void Remove(string key);

        Task RemoveAsync(string key);

        void Clear();

        Task ClearAsync();
    }
}