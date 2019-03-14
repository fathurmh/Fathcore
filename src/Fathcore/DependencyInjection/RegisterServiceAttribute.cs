using System;

namespace Fathcore.DependencyInjection
{
    /// <summary>
    /// Represents the register service attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RegisterServiceAttribute : Attribute
    {
        /// <summary>
        /// The lifetime of the service.
        /// </summary>
        public Lifetime Lifetime { get; private set; }

        public RegisterServiceAttribute() : this(Lifetime.Transient)
        {
        }

        public RegisterServiceAttribute(Lifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
