using System;
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
            if (memberExpression.Body is MemberExpression memberSelectorExpression && memberSelectorExpression.Member is PropertyInfo property && property.CanWrite)
                property.SetValue(target, value, null);

            return target;
        }
    }
}
