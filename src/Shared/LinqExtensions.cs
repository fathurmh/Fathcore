using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fathcore
{
    internal static partial class LinqExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">A strongly typed list of objects.</param>
        /// <param name="func">The <see cref="Func{T, TResult}"/> delegate to perform on each element of the <see cref="List{T}"/>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            foreach (var value in list)
            {
                await func(value);
            }
        }
    }
}
