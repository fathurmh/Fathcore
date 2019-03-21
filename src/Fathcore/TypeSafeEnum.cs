using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fathcore
{
    /// <summary>
    /// Represents type-safe enum.
    /// </summary>
    /// <typeparam name="T">The type of type safe enum.</typeparam>
    /// <typeparam name="TKey">The type of Id.</typeparam>
    public abstract class TypeSafeEnum<T, TKey> : TypeSafeEnum<T>, ITypeSafeEnum<TKey>, ITypeSafeEnum
        where T : ITypeSafeEnum<TKey>, ITypeSafeEnum
        where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        /// <summary>
        /// Gets the id value of type-safe enum.
        /// </summary>
        public TKey Id { get; }

        /// <summary>
        /// Construct the type-safe enum.
        /// </summary>
        /// <param name="id">The given id.</param>
        /// <param name="name">The given name.</param>
        /// <param name="description">The given description.</param>
        protected TypeSafeEnum(TKey id, string name, string description)
            : base(name, description)
        {
            Id = id;
        }

        /// <summary>
        /// Get value with specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>Type-safe value.</returns>
        public static T GetValue(TKey id) => GetValues().FirstOrDefault(prop => prop.Id.Equals(id));

        /// <summary>
        /// Cast value to int.
        /// </summary>
        /// <param name="typeSafeEnum">The value being casted.</param>
        public static explicit operator TKey(TypeSafeEnum<T, TKey> typeSafeEnum) => typeSafeEnum.Id;

        /// <summary>
        /// Cast int to value.
        /// </summary>
        /// <param name="id">The int being casted.</param>
        public static explicit operator TypeSafeEnum<T, TKey>(TKey id) => (dynamic)GetValue(id);
    }

    /// <summary>
    /// Represents type-safe enum.
    /// </summary>
    /// <typeparam name="T">The type of type safe enum.</typeparam>
    public abstract class TypeSafeEnum<T> : ITypeSafeEnum
        where T : ITypeSafeEnum
    {
        /// <summary>
        /// Gets the name value of type-safe enum.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description value of type-safe enum.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Construct the type-safe enum.
        /// </summary>
        /// <param name="name">The given name.</param>
        /// <param name="description">The given description.</param>
        protected TypeSafeEnum(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Convert type-safe enum to string value.
        /// </summary>
        /// <returns>Returns string value of type-safe enum.</returns>
        public override string ToString() => Name;

        /// <summary>
        /// Get name from all values.
        /// </summary>
        /// <returns>List of names.</returns>
        public static IEnumerable<string> GetNames() => GetValues().Select(prop => prop.Name);

        /// <summary>
        /// Get value with specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>Type-safe value.</returns>
        public static T GetValue(string name) => GetValues().FirstOrDefault(prop => prop.Name == name);

        /// <summary>
        /// Get all values.
        /// </summary>
        /// <returns>List of values.</returns>
        public static IReadOnlyList<T> GetValues()
        {
            // There are other ways to do that such as filling a collection in the constructor
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(property => (T)property.GetValue(null))
                .ToList();
        }

        /// <summary>
        /// Cast string to value.
        /// </summary>
        /// <param name="name">The string being casted.</param>
        public static explicit operator TypeSafeEnum<T>(string name) => (dynamic)GetValue(name);
    }
}
