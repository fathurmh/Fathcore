using System;

namespace Fathcore
{
    /// <summary>
    /// Provides the interface for strongly typed enumerations.
    /// </summary>
    public interface ITypeSafeEnum<TKey> : ITypeSafeEnum where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        /// <summary>
        /// Gets the identifier of element contained in the <see cref="ITypeSafeEnum{TKey}"/>.
        /// </summary>
        TKey Id { get; }
    }

    /// <summary>
    /// Provides the interface for type-safe enumerations.
    /// </summary>
    public interface ITypeSafeEnum
    {
        /// <summary>
        /// Gets the name of element contained in the <see cref="ITypeSafeEnum"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of element contained in the <see cref="ITypeSafeEnum"/>.
        /// </summary>
        string Description { get; }
    }
}