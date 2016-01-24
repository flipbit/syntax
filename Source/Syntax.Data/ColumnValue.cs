using System;
using System.Linq.Expressions;

namespace Syntax.Data
{
    /// <summary>
    /// Represents a single column in a table to insert / update
    /// </summary>
    public class ColumnValue
    {
        /// <summary>
        /// Gets or sets the name of the column in the table to update.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter to pass to the SQL command.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the name of the property on the underlying object.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the value to update the table with.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        /// Builds the Column Value from the given LINQ expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static ColumnValue Build<T>(object target, Expression<Func<T, object>> value)
        {
            var columnValue = new ColumnValue();

            columnValue.PropertyName = ReflectionHelper.GetExpressionName(value).ToString();
            var sqlName = columnValue.PropertyName.Replace(".", string.Empty);

            columnValue.Name = "[" + sqlName + "]";
            columnValue.ParameterName = "@" + sqlName;

            var propertyValue = ReflectionHelper.GetPropertyValue(columnValue.PropertyName, target);
            if (propertyValue == null)
            {
                columnValue.Value = DBNull.Value;
            }
            else
            {
                columnValue.Value = propertyValue;
            }

            return columnValue;
        }
    }
}
