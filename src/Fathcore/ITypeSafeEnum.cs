using System;

namespace Fathcore
{
    /// <summary>
    /// Represents type-safe enum interface.
    /// </summary>
    public interface ITypeSafeEnum<T> where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Gets the id value of type-safe enum.
        /// </summary>
        T Id { get; }

        /// <summary>
        /// Gets the name value of type-safe enum.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description value of type-safe enum.
        /// </summary>
        string Description { get; }
    }
}