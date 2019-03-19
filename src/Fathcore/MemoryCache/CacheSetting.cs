namespace Fathcore.MemoryCache
{
    /// <summary>
    /// Represents default values related to caching.
    /// </summary>
    public class CacheSetting : ICacheSetting
    {
        /// <summary>
        /// Gets a value of cache prefix.
        /// </summary>
        public string CachePrefix => "fathcore.";

        /// <summary>
        /// Gets the default cache time in minutes.
        /// </summary>
        public int CacheTime => 60;
    }
}
