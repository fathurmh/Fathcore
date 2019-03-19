using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fathcore
{
    /// <summary>
    /// Represents type-safe enum.
    /// </summary>
    /// <typeparam name="Type">The type of type safe enum.</typeparam>
    /// <typeparam name="Key">The type of Id.</typeparam>
    public class TypeSafeEnum<Type, Key> : ITypeSafeEnum<Key>
        where Type : ITypeSafeEnum<Key>
        where Key : IComparable, IComparable<Key>, IConvertible, IEquatable<Key>, IFormattable
    {
        /// <summary>
        /// Gets the id value of type-safe enum.
        /// </summary>
        public Key Id { get; }

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
        /// <param name="id">The given id.</param>
        /// <param name="name">The given name.</param>
        /// <param name="description">The given description.</param>
        protected TypeSafeEnum(Key id, string name, string description)
        {
            Id = id;
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
        /// Get value with specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>Type-safe value.</returns>
        public static Type GetValue(Key id) => GetValues().First(prop => prop.Id.Equals(id));

        /// <summary>
        /// Get value with specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>Type-safe value.</returns>
        public static Type GetValue(string name) => GetValues().First(prop => prop.Name == name);

        /// <summary>
        /// Get all values.
        /// </summary>
        /// <returns>List of values.</returns>
        public static IReadOnlyList<Type> GetValues()
        {
            // There are other ways to do that such as filling a collection in the constructor
            return typeof(Type).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(property => (Type)property.GetValue(null))
                .ToList();
        }

        /// <summary>
        /// Cast value to int.
        /// </summary>
        /// <param name="typeSafeEnum">The value being casted.</param>
        public static explicit operator Key(TypeSafeEnum<Type, Key> typeSafeEnum) => typeSafeEnum.Id;

        /// <summary>
        /// Cast int to value.
        /// </summary>
        /// <param name="id">The int being casted.</param>
        public static explicit operator TypeSafeEnum<Type, Key>(Key id) => (dynamic)GetValue(id);

        /// <summary>
        /// Cast string to value.
        /// </summary>
        /// <param name="name">The string being casted.</param>
        public static explicit operator TypeSafeEnum<Type, Key>(string name) => (dynamic)GetValue(name);
    }
}
