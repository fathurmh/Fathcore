using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Fathcore.EntityFramework.Extensions
{
    /// <summary>
    /// Linq extensions.
    /// </summary>
    internal static class QueryableExtensions
    {
        /// <summary>
        /// Specifies related entities to include in the query results. The navigation properties to be included is specified starting with the type of entity being queried (TEntity).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being queried.</typeparam>
        /// <param name="source">The source query.</param>
        /// <param name="navigationProperties">An array of lambda expression representing the navigation property to be included (t => t.Property1).</param>
        /// <returns>A new query with the related data included.</returns>
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> source, IEnumerable<Expression<Func<TEntity, object>>> navigationProperties)
            where TEntity : class
        {
            foreach (var navigationProperty in navigationProperties)
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
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> source, IEnumerable<string> navigationProperties)
            where TEntity : class
        {
            foreach (var navigationProperty in navigationProperties)
            {
                source = source.Include(navigationProperty);
            }
            return source;
        }
    }
}
