using Fathcore.Infrastructure.DependencyInjection;
using Fathcore.Infrastructure.TypeFinders;

namespace Fathcore.Infrastructure.Engines
{
    /// <summary>
    /// Provides programmatic configuration for the Engine.
    /// </summary>
    public class EngineOptions
    {
        /// <summary>
        /// Gets or sets the flag to activate registrar using <see cref="RegisterServiceAttribute"/>. Default is false.
        /// </summary>
        public bool ActivateAttributeRegistrar { get; set; }

        /// <summary>
        /// Gets or sets the flag to activate registrar that implements from <see cref="IDependencyRegistrar"/>. Default is false.
        /// </summary>
        public bool ActivateClassRegistrar { get; set; }

        /// <summary>
        /// Gets or sets the instance of <see cref="ITypeFinder"/>.
        /// </summary>
        public ITypeFinder TypeFinder { get; set; } = new TypeFinder();
    }
}
