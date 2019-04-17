using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fathcore.Infrastructure.TypeFinders
{
    /// <summary>
    /// Classes implementing this interface provide information about types to various services in the Core engine.
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// Find classes of type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Result.</returns>
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        /// <summary>
        /// Find classes of type.
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Result.</returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        /// <summary>
        /// Find classes of type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Result.</returns>
        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// Find classes of type.
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from.</param>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Result.</returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// Find classes which have specified attribute.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        IEnumerable<Type> FindClassesWithAttribute<T>(bool onlyConcreteClasses = true) where T : Attribute;

        /// <summary>
        /// Find classes which have specified attribute.
        /// </summary>
        /// <param name="attributeType">Attribute type</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        IEnumerable<Type> FindClassesWithAttribute(Type attributeType, bool onlyConcreteClasses = true);

        /// <summary>
        /// Find classes which have specified attribute.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        IEnumerable<Type> FindClassesWithAttribute<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true) where T : Attribute;

        /// <summary>
        /// Find classes which have specified attribute.
        /// </summary>
        /// <param name="attributeType">Attribute type</param>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Result.</returns>
        IEnumerable<Type> FindClassesWithAttribute(Type attributeType, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// Find all classes.
        /// </summary>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        IEnumerable<Type> FindAllClasses(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        /// <summary>
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies.</returns>
        IList<Assembly> GetAssemblies();
    }
}
