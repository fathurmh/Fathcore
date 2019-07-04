using System.Collections.Generic;
using System.Diagnostics;
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
            var replacements = new Dictionary<string, string>();
            WalkExpression(replacements, expression);

            var body = expression.Body.ToString();

            foreach (var param in expression.Parameters)
            {
                var paramName = param.Name;
                var paramTypeName = param.Type.Name;
                body = body.Replace(paramName + ".", paramTypeName + ".");
            }

            foreach (var replacement in replacements)
            {
                body = body.Replace(replacement.Key, replacement.Value);
            }

            return body;
        }

        private static void WalkExpression(Dictionary<string, string> replacements, Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var replacementExpression = expression.ToString();
                    if (replacementExpression.Contains("value("))
                    {
                        var replacementValue = Expression.Lambda(expression).Compile().DynamicInvoke().ToString();
                        if (!replacements.ContainsKey(replacementExpression))
                        {
                            replacements.Add(replacementExpression, replacementValue.ToString());
                        }
                    }
                    break;

                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                case ExpressionType.Equal:
                    var bexp = expression as BinaryExpression;
                    WalkExpression(replacements, bexp.Left);
                    WalkExpression(replacements, bexp.Right);
                    break;

                case ExpressionType.Call:
                    var mcexp = expression as MethodCallExpression;
                    foreach (var argument in mcexp.Arguments)
                    {
                        WalkExpression(replacements, argument);
                    }
                    break;

                case ExpressionType.Lambda:
                    var lexp = expression as LambdaExpression;
                    WalkExpression(replacements, lexp.Body);
                    break;

                case ExpressionType.Constant:
                    break;

                default:
                    Trace.WriteLine("Unknown type");
                    break;
            }
        }
    }
}
