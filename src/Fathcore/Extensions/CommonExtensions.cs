using System;

namespace Fathcore.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Get property value by given property name.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="propertyName">Given property name.</param>
        /// <typeparam name="T">Type of property value.</typeparam>
        /// <returns>Property value</returns>
        public static T GetPropertyValue<T>(this object source, string propertyName)
        {
            return (T)source.GetPropertyValue(propertyName);
        }

        /// <summary>
        /// Get property value by given property name.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="propertyName">Given property name.</param>
        /// <returns>Property value</returns>
        public static object GetPropertyValue(this object source, string propertyName)
        {
            return source.GetType().GetProperty(propertyName).GetValue(source);
        }
    }
}
