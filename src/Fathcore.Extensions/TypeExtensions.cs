using System;

namespace Fathcore.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether an instance of the current type can be assigned to an instance of a specified type.
        /// </summary>
        /// <param name="source">Instance of the current type.</param>
        /// <param name="destination">Instance of a specified type.</param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(this Type source, Type destination)
        {
            var interfaceTypes = source.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == destination)
                    return true;
            }

            if (source.IsGenericType && source.GetGenericTypeDefinition() == destination)
                return true;

            Type baseType = source.BaseType;
            if (baseType == null)
                return false;

            return IsAssignableToGenericType(baseType, destination);
        }
    }
}
