using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Fathcore.DependencyInjection;
using Fathcore.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Fathcore.MemoryCache
{
    /// <summary>
    /// Represents a manager for caching during an HTTP request (short term caching).
    /// </summary>
    [RegisterService(Lifetime.Scoped)]
    public class ScopedCacheManager : ICacheManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheSetting _cacheSetting;
        private readonly ReaderWriterLockSlim _locker;

        public ScopedCacheManager(IHttpContextAccessor httpContextAccessor, ICacheSetting cacheSetting)
        {
            _httpContextAccessor = httpContextAccessor;
            _cacheSetting = cacheSetting;
            _locker = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Gets a key/value collection that can be used to share data within the scope of this request.
        /// </summary>
        protected virtual IDictionary<object, object> GetItems()
        {
            return _httpContextAccessor.HttpContext?.Items;
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
            using (new ReaderWriteLock(_locker, false))
            {
                var items = GetItems();
                if (items == null)
                    return acquire();

                if (items[key] != null)
                    return (T)items[key];

                var result = acquire();

                if (result != null && (cacheTime ?? _cacheSetting.CacheTime) > 0)
                    items[key] = result;

                return result;
            }
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <param name="data">Value for caching.</param>
        /// <param name="cacheTime">Cache time in minutes.</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            using (new ReaderWriteLock(_locker, true))
            {
                var items = GetItems();
                if (items == null)
                    return;

                if (data != null)
                    items[key] = data;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        /// <returns>True if item already is in cache; otherwise false.</returns>
        public virtual bool IsSet(string key)
        {
            using (new ReaderWriteLock(_locker, false))
            {
                var items = GetItems();

                return items?[key] != null;
            }
        }

        /// <summary>
        /// Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key">Key of cached item.</param>
        public virtual void Remove(string key)
        {
            using (new ReaderWriteLock(_locker, true))
            {
                var items = GetItems();

                items?.Remove(key);
            }
        }

        /// <summary>
        /// Removes items by key pattern.
        /// </summary>
        /// <param name="pattern">String key pattern.</param>
        public virtual void RemoveByPattern(string pattern)
        {
            using (new ReaderWriteLock(_locker, true))
            {
                var items = GetItems();
                if (items == null)
                    return;

                var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matchesKeys = items.Keys.Select(p => p.ToString()).Where(key => regex.IsMatch(key)).ToList();

                foreach (var key in matchesKeys)
                    items.Remove(key);
            }
        }

        /// <summary>
        /// Clear all cache data.
        /// </summary>
        public virtual void Clear()
        {
            using (new ReaderWriteLock(_locker, true))
            {
                var items = GetItems();

                items?.Clear();
            }
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
