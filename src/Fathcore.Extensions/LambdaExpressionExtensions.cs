using System.Linq.Expressions;

namespace Fathcore.Extensions
{
    /// <summary>
    /// Extension methods for Lambda Expression.
    /// </summary>
    public static class LambdaExpressionExtensions
    {
        /// <summary>
        /// Get readable expression body.
        /// </summary>
        /// <param name="expression">Describes a lambda expression.</param>
        /// <returns></returns>
        public static string GetBodyString(this LambdaExpression expression)
        {
            var result = expression.Body.ToString();
            var paramName = expression.Parameters[0].Name;
            var paramTypeName = expression.Parameters[0].Type.Name;

            result = result.Replace(paramName + ".", paramTypeName + ".")
                        .Replace("AndAlso", "&&")
                        .Replace("OrElse", "||");

            return result;
        }
    }
}
