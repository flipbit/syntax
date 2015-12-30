using System;
using System.Linq;
using System.Linq.Expressions;

namespace Syntax.Data
{
    public class ReflectionHelper
    {
        public static object GetExpressionName<T>(Expression<Func<T, object>> expression)
        {
            return GetExpressionName(expression.Body);

        }

        public static object GetExpressionName(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                var name = memberExpression.Member.Name;

                if (memberExpression.Expression is MemberExpression)
                {
                    var parent = GetExpressionName(memberExpression.Expression);

                    if (parent != null)
                    {
                        name = parent + "." + name;
                    }
                }

                return name;
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                return GetExpressionName(unaryExpression.Operand);
            }

            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
            {
                return constantExpression.Value;
            }

            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                return GetExpressionName(lambdaExpression.Body);
            }

            throw new ArgumentException("Can't get name of: " + expression.GetType());
        }

        public static string GetExpressionType(Expression expression)
        {
            var body = expression as MemberExpression;

            return body.Expression.Type.Name;
        }

        public static object GetExpressionValue(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        /// <summary>
        /// Gets the operand string for the given <see cref="ExpressionType"/>.
        /// </summary>
        /// <param name="expressionType">Type of the node.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Unsupported Expression Node Type:  + nodeType</exception>
        public static string GetSqlOperand(ExpressionType expressionType)
        {
            string result;

            switch (expressionType)
            {
                case ExpressionType.Equal:
                    result = "=";
                    break;

                case ExpressionType.NotEqual:
                    result = "!=";
                    break;

                case ExpressionType.LessThan:
                    result = "<";
                    break;

                case ExpressionType.LessThanOrEqual:
                    result = "<=";
                    break;

                case ExpressionType.GreaterThan:
                    result = ">";
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    result = ">=";
                    break;

                default:
                    throw new ArgumentException("Unsupported Expression Type: " + expressionType);
            }

            return result;
        }

        public static object GetPropertyValue(string name, object value)
        {
            object result = null;

            if (value == null) return result;

            var properties = value.GetType().GetProperties();

            var segments = name.Split('.');

            var segment = segments.First();

            foreach (var property in properties)
            {
                if (property.Name == segment)
                {
                    result = property.GetValue(value);

                    if (segments.Length > 1)
                    {
                        result = GetPropertyValue(name.SubstringAfterString("."), result);
                    }

                    break;
                }
            }

            return result;
        }
    }
}