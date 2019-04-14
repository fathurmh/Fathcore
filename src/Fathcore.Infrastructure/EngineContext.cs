using System.Runtime.CompilerServices;

namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Provides access to the singleton instance of the engine.
    /// </summary>
    public static class EngineContext
    {
        /// <summary>
        /// Create a static instance of the engine.
        /// </summary>
        /// <returns>The engine to use.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create()
        {
            return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new Engine());
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        /// <summary>
        /// Gets the singleton engine used to access services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                    Create();

                return Singleton<IEngine>.Instance;
            }
        }
    }
}
