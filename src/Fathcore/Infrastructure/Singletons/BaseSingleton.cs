using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Provides access to all "singletons" stored by <see cref="Singleton{T}"/>.
    /// </summary>
    public abstract class BaseSingleton
    {
        static BaseSingleton()
        {
            AllSingletons = new ConcurrentDictionary<Type, object>();
        }

        /// <summary>
        /// Dictionary of type to singleton instances.
        /// </summary>
        public static IDictionary<Type, object> AllSingletons { get; }
    }
}
