using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Fathcore.MemoryCache
{
    /// <summary>
    /// Represents a memory cache manager.
    /// </summary>
    public class MemoryCacheManager : ILocker, IStaticCacheManager
    {
        private readonly IMemoryCache _cache;
        private readonly ICacheSetting _cacheSetting;

        /// <summary>
        /// All keys of cache.
        /// </summary>
        /// <remarks>Dictionary value indicating whether a key still exists in cache.</remarks> 
        protected static readonly ConcurrentDictionary<string, bool> s_allKeys;

        /// <summary>
        /// Cancellation token for clear cache.
        /// </summary>
        protected CancellationTokenSource _cancellationTokenSource;

        static MemoryCacheManager()
        {
            s_allKeys = new ConcurrentDictionary<string, bool>();
        }

        public MemoryCacheManager(IMemoryCache cache, ICacheSetting cacheSetting)
        {
            _cache = cache;
            _cacheSetting = cacheSetting;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Create entry options to item of memory cache.
        /// </summary>
        /// <param name="cacheTime">Cache time.</param>
        protected MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TimeSpan cacheTime)
        {
            var options = new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
                .RegisterPostEvictionCallback(PostEviction);

            options.AbsoluteExpirationRelativeToNow = cacheTime;

            return options;
        }

        /// <summary>
        /// Add key to dictionary.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <returns>Itself key.</returns>
        protected string AddKey(string key)
        {
            s_allKeys.TryAdd(key, true);
            return key;
        }

        /// <summary>
        /// Remove key from dictionary.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <returns>Itself key.</returns>
        protected string RemoveKey(string key)
        {
            TryRemoveKey(key);
            return key;
        }

        /// <summary>
        /// Try to remove a key from dictionary, or mark a key as not existing in cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        protected void TryRemoveKey(string key)
        {
            if (!s_allKeys.TryRemove(key, out _))
                s_allKeys.TryUpdate(key, false, true);
        }

        /// <summary>
        /// Remove all keys marked as not existing.
        /// </summary>
        private void ClearKeys()
        {
            foreach (var key in s_allKeys.Where(p => !p.Value).Select(p => p.Key).ToList())
                RemoveKey(key);
        }

        /// <summary>
        /// Post eviction.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <param name="value">Value of cached item.</param>
        /// <param name="reason">Eviction reason.</param>
        /// <param name="state">State.</param>
        private void PostEviction(object key, object value, EvictionReason reason, object state)
        {
            if (reason == EvictionReason.Replaced)
                return;

            ClearKeys();
            TryRemoveKey(key.ToString());
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it.
        /// </summary>
        /// <typeparam name="T">Type of cached item.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet.</param>
        /// <param name="cacheTime">Cache time in minutes; pass 0 to do not cache; pass null to use the default time.</param>
        /// <returns>The cached value associated with the specified key.</returns>
        public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            if (_cache.TryGetValue(key, out T value))
                return value;

            var result = acquire();

            if ((cacheTime ?? _cacheSetting.CacheTime) > 0)
                Set(key, result, cacheTime ?? _cacheSetting.CacheTime);

            return result;
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it.
        /// </summary>
        /// <typeparam name="T">Type of cached item.</typeparam>
        /// <param name="key">Cache key.</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet.</param>
        /// <param name="cacheTime">Cache time in minutes; pass 0 to do not cache; pass null to use the default time.</param>
        /// <returns>The cached value associated with the specified key.</returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            if (_cache.TryGetValue(key, out T value))
                return value;

            var result = await acquire();

            if ((cacheTime ?? _cacheSetting.CacheTime) > 0)
                Set(key, result, cacheTime ?? _cacheSetting.CacheTime);

            return result;
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <param name="data">Value for caching.</param>
        /// <param name="cacheTime">Cache time in minutes.</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data != null)
                _cache.Set(AddKey(key), data, GetMemoryCacheEntryOptions(TimeSpan.FromMinutes(cacheTime)));
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <returns>True if item already is in cache; otherwise false.</returns>
        public virtual bool IsSet(string key)
        {
            return _cache.TryGetValue(key, out object _);
        }

        /// <summary>
        /// Perform some action with exclusive in-memory lock.
        /// </summary>
        /// <param name="key">The key we are locking on.</param>
        /// <param name="expirationTime">The time after which the lock will automatically be expired.</param>
        /// <param name="action">Action to be performed with locking.</param>
        /// <returns>True if lock was acquired and action was performed; otherwise false.</returns>
        public bool PerformActionWithLock(string key, TimeSpan expirationTime, Action action)
        {
            if (!s_allKeys.TryAdd(key, true))
                return false;

            try
            {
                _cache.Set(key, key, GetMemoryCacheEntryOptions(expirationTime));
                action();

                return true;
            }
            finally
            {
                Remove(key);
            }
        }

        /// <summary>
        /// Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        public virtual void Remove(string key)
        {
            _cache.Remove(RemoveKey(key));
        }

        /// <summary>
        /// Removes items by key pattern.
        /// </summary>
        /// <param name="pattern">String key pattern.</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = s_allKeys.Where(p => p.Value).Select(p => p.Key).Where(key => regex.IsMatch(key)).ToList();

            foreach (var key in matchesKeys)
                _cache.Remove(RemoveKey(key));
        }

        /// <summary>
        /// Clear all cache data.
        /// </summary>
        public virtual void Clear()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Dispose cache manager.
        /// </summary>
        public virtual void Dispose()
        {
            //nothing special
        }
    }
}
