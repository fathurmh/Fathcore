namespace Fathcore.MemoryCache
{
    /// <summary>
    /// Represents default values related to caching.
    /// </summary>
    public interface ICacheSetting
    {
        /// <summary>
        /// Gets a value of cache prefix.
        /// </summary>
        string CachePrefix { get; }

        /// <summary>
        /// Gets the default cache time in minutes.
        /// </summary>
        int CacheTime { get; }
    }
}
