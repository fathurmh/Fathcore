using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Fathcore.Extensions
{
    /// <summary>
    /// Object extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Set property value with lambda expression.
        /// </summary>
        /// <param name="target">Target object being set.</param>
        /// <param name="memberExpression">Target member property expression.</param>
        /// <param name="value">Value</param>
        /// <typeparam name="T">Type of target.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <returns></returns>
        public static T SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberExpression, TValue value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (memberExpression == null)
                throw new ArgumentNullException(nameof(memberExpression));

            if (memberExpression.Body is MemberExpression memberSelectorExpression && memberSelectorExpression.Member is PropertyInfo property && property.CanWrite)
                property.SetValue(target, value, null);

            return target;
        }

        /// <summary>
        /// Returns the property value of a specified object and property name.
        /// </summary>
        /// <typeparam name="T">Expected type of property value.</typeparam>
        /// <param name="source">The object whose property value will be returned.</param>
        /// <param name="propertyName">The string containing the name of the public property to get.</param>
        /// <returns>The property value of the specified property name.</returns>
        public static T GetPropertyValue<T>(this object source, string propertyName)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            return (T)source.GetPropertyValue(propertyName);
        }

        /// <summary>
        /// Returns the property value of a specified object and property name.
        /// </summary>
        /// <param name="source">The object whose property value will be returned.</param>
        /// <param name="propertyName">The string containing the name of the public property to get.</param>
        /// <returns>The property value of the specified property name.</returns>
        public static object GetPropertyValue(this object source, string propertyName)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            return source.GetType().GetProperty(propertyName).GetValue(source);
        }

        /// <summary>
        /// Convert objects as enumerable.
        /// </summary>
        /// <param name="source">Items to convert.</param>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <returns>IEnumerable object.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T source) where T : new()
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.AsEnumerableIterator();
        }

        /// <summary>
        /// Convert objects as enumerable.
        /// </summary>
        /// <param name="source">Items to convert.</param>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <returns>IEnumerable object.</returns>
        private static IEnumerable<T> AsEnumerableIterator<T>(this T source) where T : new()
        {
            yield return source;
        }
    }
}
