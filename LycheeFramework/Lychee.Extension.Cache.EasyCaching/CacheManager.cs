using EasyCaching.Core;
using Lychee.Extension.Cache.Abstractions;
using System;
using System.Threading.Tasks;

namespace Lychee.Extension.Cache.EasyCaching
{
    public class CacheManager : ICache
    {
        private readonly IEasyCachingProvider cachingProvider;

        public CacheManager(IEasyCachingProvider cachingProvider)
        {
            this.cachingProvider = cachingProvider;
        }

        public void Clear()
        {
            this.cachingProvider.Flush();
        }

        public Task ClearAsync()
        {
            return this.cachingProvider.FlushAsync();
        }

        public bool Exists(string key)
        {
            return this.cachingProvider.Exists(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return this.cachingProvider.ExistsAsync(key);
        }

        public T Get<T>(string key, Func<T> func, TimeSpan? expiration = null)
        {
            var cacheResult = this.cachingProvider.Get(key, func, GetExpiration(expiration));
            return cacheResult.Value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null)
        {
            var cacheResult = await this.cachingProvider.GetAsync<T>(key, func, GetExpiration(expiration));
            return cacheResult.Value;
        }

        public void Remove(string key)
        {
            this.cachingProvider.Remove(key);
        }

        public Task RemoveAsync(string key)
        {
            return this.cachingProvider.RemoveAsync(key);
        }

        public bool TryAdd<T>(string key, T Value, TimeSpan? expiration = null)
        {
            return this.cachingProvider.TrySet(key, Value, GetExpiration(expiration));
        }

        public Task<bool> TryAddAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            return this.cachingProvider.TrySetAsync(key, value, GetExpiration(expiration));
        }

        private TimeSpan GetExpiration(TimeSpan? expiration)
        {
            expiration ??= TimeSpan.FromHours(12);
            return expiration.Value;
        }
    }
}