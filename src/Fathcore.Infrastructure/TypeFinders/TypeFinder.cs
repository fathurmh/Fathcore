using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fathcore.Infrastructure.TypeFinders
{
    /// <summary>
    /// A class that finds types needed by looping assemblies in the 
    /// currently executing AppDomain. Only assemblies whose names matches
    /// certain patterns are investigated and an optional list of assemblies
    /// referenced by <see cref="AssemblyNames"/> are always investigated.
    /// </summary>
    public class TypeFinder : ITypeFinder
    {
        private readonly bool _ignoreReflectionErrors = true;

        /// <summary>
        /// Gets or sets whether should iterate assemblies in the app domain when loading types. Loading patterns are applied when loading these assemblies.
        /// </summary>
        public bool LoadAppDomainAssemblies { get; set; } = true;

        /// <summary>
        /// Gets or sets assemblies loaded a startup in addition to those loaded in the AppDomain.
        /// </summary>
        public IList<string> AssemblyNames { get; set; } = new List<string>();

        /// <summary>
        /// Gets the pattern for dlls that we know don't need to be investigated.
        /// </summary>
        public string AssemblySkipLoadingPattern { get; set; } = "^System|^mscorlib|^Microsoft|^Newtonsoft|^SQLitePCLRaw|^xunit|^Remotion|^StackExchange|^MessagePack|^Moq|^Castle|^*.Tests";

        /// <summary>
        /// Gets or sets the pattern for dll that will be investigated.
        /// For ease of use this defaults to match all but to increase performance you might want to configure a pattern that includes assemblies and your own.
        /// </summary>
        /// <remarks>
        /// If you change this so that assemblies aren't investigated (e.g. by not including something like "^Core|..." you may break core functionality.
        /// </remarks>
        public string AssemblyRestrictToLoadingPattern { get; set; } = ".*";

        /// <summary>
        /// Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Distinct())
            {
                if (!Matches(assembly.FullName))
                    continue;

                assemblies.Add(assembly);
                addedAssemblyNames.Add(assembly.FullName);
            }
        }

        /// <summary>
        /// Adds specifically configured assemblies.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assemblyName in AssemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);
                if (addedAssemblyNames.Contains(assembly.FullName))
                    continue;

                assemblies.Add(assembly);
                addedAssemblyNames.Add(assembly.FullName);
            }
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName"> The name of the assembly to check.</param>
        /// <returns>True if the assembly should be loaded.</returns>
        public virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern)
                   && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>
        /// Check if a dll is one of the shipped dlls that we know don't need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">The assembly name to match.</param>
        /// <param name="pattern">The regular expression pattern to match against the assembly name.</param>
        /// <returns>True if the pattern matches the assembly name.</returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Does type implement generic?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <returns></returns>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                Type genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (Type implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Find classes of type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        public virtual IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes of type.
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        public virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes of type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        public virtual IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes of type.
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from.</param>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        public virtual IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = FindAllClasses(assemblies, onlyConcreteClasses)
                .Where(type => assignTypeFrom.IsAssignableFrom(type) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(type, assignTypeFrom)));

            return result;
        }

        /// <summary>
        /// Find classes which have specified attribute.
        /// </summary>
        /// <typeparam name="T">Attribute type.</typeparam>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        public virtual IEnumerable<Type> FindClassesWithAttribute<T>(bool onlyConcreteClasses = true) where T : Attribute
        {
            return FindClassesWithAttribute(typeof(T), onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes which have specified attribute.
        /// </summary>
        /// <param name="attributeType">Attribute type</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        public virtual IEnumerable<Type> FindClassesWithAttribute(Type attributeType, bool onlyConcreteClasses = true)
        {
            return FindClassesWithAttribute(attributeType, GetAssemblies(), onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes which have specified attribute.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        public virtual IEnumerable<Type> FindClassesWithAttribute<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true) where T : Attribute
        {
            return FindClassesWithAttribute(typeof(T), assemblies, onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes which have specified attribute.
        /// </summary>
        /// <param name="attributeType">Attribute type</param>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Result.</returns>
        public virtual IEnumerable<Type> FindClassesWithAttribute(Type attributeType, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = FindAllClasses(assemblies, onlyConcreteClasses)
                .Where(type => type.GetCustomAttributes().Any(attr => attr.GetType() == attributeType));

            return result;
        }

        /// <summary>
        /// Find all classes.
        /// </summary>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes.</param>
        /// <returns>Type collection.</returns>
        public virtual IEnumerable<Type> FindAllClasses(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();
            var partitioner = Partitioner.Create(assemblies, EnumerablePartitionerOptions.NoBuffering);
            var parallelOption = new ParallelOptions { MaxDegreeOfParallelism = 2 };

            Parallel.ForEach(partitioner, parallelOption, assembly =>
            {
                Type[] types = null;
                try
                {
                    types = assembly.GetTypes();
                }
                catch
                {
                    if (!_ignoreReflectionErrors)
                        throw;
                }

                if (types == null)
                    return;

                var nestedPartitioner = Partitioner.Create(types, EnumerablePartitionerOptions.NoBuffering);
                Parallel.ForEach(nestedPartitioner, parallelOption, type =>
                {
                    if (type.IsInterface)
                        return;

                    if (onlyConcreteClasses)
                    {
                        if (type.IsClass && !type.IsAbstract)
                        {
                            result.Add(type);
                        }
                    }
                    else
                    {
                        result.Add(type);
                    }
                });
            });

            return result;
        }

        /// <summary>
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies.</returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
                AddAssembliesInAppDomain(addedAssemblyNames, assemblies);
            AddConfiguredAssemblies(addedAssemblyNames, assemblies);

            return assemblies;
        }
    }
}
