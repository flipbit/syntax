using System;
using System.Linq.Expressions;

namespace Syntax.Data
{
    public class Order
    {
        public string ColumnName { get; set; }

        public static Order Build<T>(Expression<Func<T, object>> expression)
        {
            var unaryExpression = expression.Body as UnaryExpression;

            if (unaryExpression == null)
            {
                throw new ArgumentException("Can't create clause from expression type: " + expression.Body.GetType().Name);
            }

            return Build(unaryExpression);
        }

        private static Order Build(UnaryExpression expression)
        {
            var memberExpression = expression.Operand as MemberExpression;
            if (memberExpression != null)
            {
                var order = new Order();

                order.ColumnName = GetQualifiedPropertyName(memberExpression);

                return order;
            }

            var unaryExpression = expression.Operand as UnaryExpression;
            if (unaryExpression != null)
            {
                return Build(unaryExpression);
            }

            throw new ArgumentException("Can't create Clause from expression type: " + expression.GetType());
        }

        private static string GetQualifiedPropertyName(Expression expression)
        {
            var className = ReflectionHelper.GetExpressionType(expression);
            var propertyName = ReflectionHelper.GetExpressionName(expression);

            return string.Format("[{0}].[{1}]", className, propertyName);
        }
    }
}
