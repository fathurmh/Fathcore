using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.Extensions
{
    public static partial class LinqExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">A strongly typed list of objects.</param>
        /// <param name="func">The <see cref="Func{T, TResult}"/> delegate to perform on each element of the <see cref="List{T}"/>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            foreach (var value in list)
            {
                await func(value);
            }
        }

        /// <summary>
        /// Specifies related entities to include in the query results. The navigation properties to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="navigationProperties">An array of lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A new query with the related data included.</returns>
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, object>>[] navigationProperties)
            where TEntity : class
        {
            foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
            {
                source = source.Include(navigationProperty);
            }
            return source;
        }

        /// <summary>
        /// Specifies related entities to include in the query results. The navigation property to be included is specified starting with the type of entity being queried (TEntity).
        /// Further navigation properties to be included can be appended, separated by the '.' character.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="navigationProperties">An array of string, a string of '.' separated navigation property names to be included.</param>
        /// <returns>A new query with the related data included.</returns>
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> source, string[] navigationProperties)
            where TEntity : class
        {
            foreach (string navigationProperty in navigationProperties)
            {
                source = source.Include(navigationProperty);
            }
            return source;
        }
    }
}
