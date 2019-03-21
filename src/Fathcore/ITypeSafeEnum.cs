using System;

namespace Fathcore
{
    /// <summary>
    /// Represents type-safe enum interface.
    /// </summary>
    public interface ITypeSafeEnum<TKey> : ITypeSafeEnum where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        /// <summary>
        /// Gets the id value of type-safe enum.
        /// </summary>
        TKey Id { get; }
    }

    /// <summary>
    /// Represents type-safe enum interface.
    /// </summary>
    public interface ITypeSafeEnum
    {
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