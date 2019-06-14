using System;
using System.Linq;
using System.Reflection;

namespace Fathcore.Infrastructure.Enum
{
    /// <summary>
    /// Provides the base class for a strongly typed enumerations.
    /// </summary>
    /// <typeparam name="T">The type of element constant in the strongly typed enumerations.</typeparam>
    /// <typeparam name="TKey">The type of key identifier in the strongly typed enumerations.</typeparam>
    public abstract class TypeSafeEnum<T, TKey> : TypeSafeEnum<T>, ITypeSafeEnum<TKey>, ITypeSafeEnum
        where T : ITypeSafeEnum<TKey>, ITypeSafeEnum
        where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        /// <summary>
        /// Gets the identifier of element contained in the <see cref="TypeSafeEnum{T, TKey}"/>.
        /// </summary>
        public TKey Id { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSafeEnum{T, TKey}"/> class.
        /// </summary>
        /// <param name="id">The identifier of a particular enumerated constant.</param>
        /// <param name="name">The name of a particular enumerated constant.</param>
        /// <param name="description">The description of a particular enumerated constant.</param>
        protected TypeSafeEnum(TKey id, string name, string description)
            : base(name, description)
        {
            Id = id;
        }

        /// <summary>
        /// Retrieves the value of the constant in a specified strongly typed enumeration that has the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of a particular enumerated constant.</param>
        /// <returns>The value that contains the specified identifier of the constant in strongly typed enumerations; or null if no such constant is found.</returns>
        public static T GetValue(TKey id) => GetValues().FirstOrDefault(prop => prop.Id.Equals(id));

        /// <summary>
        /// Convert the value of the constant in strongly typed enumerations to specified identifier of a particular enumerated constant.
        /// </summary>
        /// <param name="enumType">The value of constant in strongly typed enumerations.</param>
        public static explicit operator TKey(TypeSafeEnum<T, TKey> enumType) => enumType.Id;

        /// <summary>
        /// Convert the specified identifier of a particular enumerated constant to the value of the constant in a specified strongly typed enumeration; or null if no such constant is found.
        /// </summary>
        /// <param name="id">The identifier of a particular enumerated constant.</param>
        public static explicit operator TypeSafeEnum<T, TKey>(TKey id) => (dynamic)GetValue(id);
    }

    /// <summary>
    /// Provides the base class for a strongly typed enumerations.
    /// </summary>
    /// <typeparam name="T">The type of element constant in the strongly typed enumerations.</typeparam>
    public abstract class TypeSafeEnum<T> : TypeSafeEnum, ITypeSafeEnum
        where T : ITypeSafeEnum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSafeEnum{T}"/> class.
        /// </summary>
        /// <param name="name">The name of a particular enumerated constant.</param>
        /// <param name="description">The description of a particular enumerated constant.</param>
        protected TypeSafeEnum(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Retrieves an array of the names of the constants in a specified strongly typed enumeration.
        /// </summary>
        /// <returns>A string array of the names of the constants in strongly typed enumeration.</returns>
        public static string[] GetNames() => GetValues().Select(prop => prop.Name).ToArray();

        /// <summary>
        /// Retrieves the value of the constant in a specified strongly typed enumeration that has the specified name.
        /// </summary>
        /// <param name="name">The name of a particular enumerated constant.</param>
        /// <returns>The value that contains the specified name of the constant in strongly typed enumerations; or null if no such constant is found.</returns>
        public static T GetValue(string name) => GetValues().FirstOrDefault(prop => prop.Name == name);

        /// <summary>
        /// Retrieves an array of the values of the constants in a specified enumeration.
        /// </summary>
        /// <returns>An array that contains the values of the constants in strongly typed enumeration.</returns>
        public static T[] GetValues()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(property => (T)property.GetValue(null))
                .ToArray();
        }

        /// <summary>
        /// Convert the specified name of a particular enumerated constant to the value of the constant in a specified strongly typed enumeration; or null if no such constant is found.
        /// </summary>
        /// <param name="name">The name of a particular enumerated constant.</param>
        public static explicit operator TypeSafeEnum<T>(string name) => (dynamic)GetValue(name);
    }

    /// <summary>
    /// Provides the base class for a strongly typed enumerations.
    /// </summary>
    public abstract class TypeSafeEnum : ITypeSafeEnum
    {
        /// <summary>
        /// Gets the name of element contained in the <see cref="TypeSafeEnum"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of element contained in the <see cref="TypeSafeEnum"/>.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSafeEnum{T}"/> class.
        /// </summary>
        /// <param name="name">The name of a particular enumerated constant.</param>
        /// <param name="description">The description of a particular enumerated constant.</param>
        protected TypeSafeEnum(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Convert the value of the constant in strongly typed enumerations to its equivalent string representation.
        /// </summary>
        /// <param name="enumType">The value of constant in strongly typed enumerations.</param>
        public static explicit operator string(TypeSafeEnum enumType) => enumType.ToString();

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString() => Name;
    }
}
