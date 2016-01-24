using System;
using System.Linq.Expressions;

namespace Syntax.Data
{
    /// <summary>
    /// Represents a SQL WHERE clause
    /// </summary>
    public class Clause
    {
        /// <summary>
        /// Gets or sets the left hand side.
        /// </summary>
        /// <value>
        /// The left hand side.
        /// </value>
        public string Left { get; set; }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        public string Operand { get; set; }

        /// <summary>
        /// Gets or sets the right hand side.
        /// </summary>
        /// <value>
        /// The right hand side.
        /// </value>
        public string Right { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has parameter value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has parameter value; otherwise, <c>false</c>.
        /// </value>
        public bool HasParameterValue { get; set; }

        /// <summary>
        /// Builds a WHERE clause from the specified expression.
        /// </summary>
        /// <param name="expression">The member.</param>
        /// <returns></returns>
        public static Clause Build<T>(Expression<Func<T, object>> expression)
        {
            var unaryExpression = expression.Body as UnaryExpression;

            if (unaryExpression == null)
            {
                throw new ArgumentException("Can't create clause from expression type: " + expression.Body.GetType().Name);
            }

            return Build(unaryExpression);
        }

        private static Clause Build(UnaryExpression expression)
        {
            var operand = expression.Operand as BinaryExpression;
            if (operand != null)
            {
                var clause = new Clause();

                clause.Left = GetQualifiedPropertyName(operand.Left); 
                clause.Operand = ReflectionHelper.GetSqlOperand(operand.NodeType);
                clause.Right = ReflectionHelper.GetExpressionValue(operand.Right).ToSqlQuotedString();

                // Handle Null Value Syntax
                if (clause.Right == "NULL")
                {
                    if (clause.Operand == "=")
                    {
                        clause.Operand = "IS";
                    }

                    if (clause.Operand == "!=")
                    {
                        clause.Operand = "IS NOT";
                    }
                }

                return clause;
            }

            var memberExpression = expression.Operand as MemberExpression;
            if (memberExpression != null)
            {
                var clause = new Clause();

                clause.Left = GetQualifiedPropertyName(memberExpression);
                clause.Operand = "=";
                clause.Right = expression.NodeType == ExpressionType.Not ? "0" : "1";

                return clause;
            }

            var unaryExpression = expression.Operand as UnaryExpression;
            if (unaryExpression != null)
            {
                return Build(unaryExpression);
            }

            Console.WriteLine(expression.Operand.GetType());

            throw new ArgumentException("Can't create Clause from expression type: " + expression.GetType());
        }

        private static string GetQualifiedPropertyName(Expression expression)
        {
            var className = ReflectionHelper.GetExpressionType(expression);
            var propertyName = ReflectionHelper.GetExpressionName(expression);

            return string.Format("[{0}].[{1}]", className, propertyName);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Left, Operand, Right);
        }
    }
}
