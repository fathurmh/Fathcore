using System;

namespace Fathcore.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Indicates that a class will be registered as a service. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class RegisterServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets the lifetime of service in the <see cref="RegisterServiceAttribute"/>.
        /// </summary>
        public Lifetime Lifetime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterServiceAttribute"/> class.
        /// </summary>
        public RegisterServiceAttribute() : this(Lifetime.Transient)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterServiceAttribute"/> class.
        /// </summary>
        /// <param name="lifetime">The lifetime of a particular service.</param>
        public RegisterServiceAttribute(Lifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
