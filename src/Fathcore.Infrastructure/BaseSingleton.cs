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
        private static readonly object s_padlock = new object();
        private static ConcurrentDictionary<Type, object> s_allSingletons;

        static BaseSingleton() { }

        /// <summary>
        /// Dictionary of type to singleton instances.
        /// </summary>
        public static IDictionary<Type, object> AllSingletons
        {
            get
            {
                lock (s_padlock)
                {
                    if (s_allSingletons == null)
                    {
                        s_allSingletons = new ConcurrentDictionary<Type, object>();
                    }

                    return s_allSingletons;
                }
            }
        }
    }
}
