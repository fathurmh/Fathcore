using System.Runtime.CompilerServices;
using Fathcore.Abstractions;
using Fathcore.Infrastructures;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore
{
    /// <summary>
    /// Provides access to the singleton instance of the Core engine.
    /// </summary>
    public class EngineContext
    {
        /// <summary>
        /// Create a static instance of the Core engine.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create(IServiceCollection services = null)
        {
            //create CoreEngine as engine
            return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new CoreEngine(services));
        }
        
        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        /// <summary>
        /// Gets the singleton Core engine used to access Core services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Create();
                }

                return Singleton<IEngine>.Instance;
            }
        }
    }
}
