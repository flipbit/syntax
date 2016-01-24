using System;
using System.Collections.Generic;
using System.Text;

namespace Syntax.Data.Dialects
{
    /// <summary>
    /// The base SQL dialect
    /// </summary>
    public abstract class SqlDialect : ISqlDialect
    {
        /// <summary>
        /// Generates an SQL INSERT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public virtual string Insert<T>(SqlInsert<T> command)
        {
            var builder = new StringBuilder();

            builder.Append("INSERT INTO ");
            builder.Append(command.TableName);
            builder.AppendLine();
            builder.Append("(");

            var first = true;
            foreach (var column in command.Columns)
            {
                if (!first) builder.Append(", ");
                builder.Append(column.Name);
                first = false;
            }

            builder.Append(")");
            builder.AppendLine();
            builder.Append("VALUES (");

            first = true;
            foreach (var column in command.Columns)
            {
                if (!first) builder.Append(", ");
                builder.Append(column.ParameterName);
                builder.Append("");
                first = false;
            }

            builder.AppendLine(");");

            return builder.ToString();
        }

        /// <summary>
        /// Generates a SQL SELECT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public virtual string Select<T>(SqlSelect<T> command)
        {
            var builder = new StringBuilder();

            builder.Append("SELECT ");

            var first = true;
            foreach (var column in command.Columns)
            {
                if (!first) { builder.Append(", "); }

                builder.AppendFormat("{0}", column.Name);
                first = false;
            }
            builder.AppendLine();

            builder
                .AppendFormat("FROM {0} {1}", command.TableName, command.WithNoLock ? "WITH(NOLOCK)" : "")
                .AppendLine();

            first = true;
            foreach (var whereClause in command.Clauses)
            {
                if (first)
                {
                    builder.Append("WHERE ");
                }
                else
                {
                    builder.Append("AND ");
                }
                builder.AppendLine(whereClause.ToString());

                first = false;
            }

            first = true;
            foreach (var order in command.Orders)
            {
                if (first)
                {
                    builder.Append("ORDER BY ");
                }
                else
                {
                    builder.Append(", ");
                }

                builder.Append(order.ColumnName);

                first = false;
            }


            return builder.ToString();
        }

        /// <summary>
        /// Generates a SQL UPDATE statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string Update<T>(SqlUpdate<T> command)
        {
            var builder = new StringBuilder();

            builder.Append("UPDATE ");
            builder.Append(command.TableName);
            builder.Append("");
            builder.AppendLine();
            builder.AppendLine("SET");

            var first = true;
            foreach (var column in command.Columns)
            {
                if (!first)
                {
                    builder.AppendLine(",");
                }
                else
                {
                    builder.AppendLine("");
                }
                builder.Append(column.Name);
                builder.Append(" = ");
                builder.Append(column.ParameterName);
                first = false;
            }

            builder.AppendLine();

            first = true;
            foreach (var whereClause in command.Clauses)
            {
                if (first) builder.Append("WHERE ");
                builder.AppendLine(whereClause.ToString());
                first = false;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates a SQL DELETE statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string Delete<T>(SqlDelete<T> command)
        {
            var builder = new StringBuilder();

            builder.Append("DELETE ");
            builder.Append(command.TableName);
            builder.AppendLine();

            var first = true;
            foreach (var whereClause in command.Clauses)
            {
                if (first) builder.Append("WHERE ");
                builder.AppendLine(whereClause.ToString());
                first = false;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generates a bulk SQL INSERT statement.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TList">The type of the list.</typeparam>
        /// <param name="command">The command.</param>
        public virtual string BulkInsert<TItem, TList>(SqlBulkInsert<TItem, TList> command) where TList : IEnumerable<TItem>
        {
            var builder = new StringBuilder();

            builder.Append("INSERT INTO ");
            builder.Append(command.TableName);
            builder.AppendLine();
            builder.Append("(");

            var first = true;
            foreach (var column in command.Columns)
            {
                if (!first) builder.Append(", ");
                builder.Append(column.Name);
                first = false;
            }

            builder.Append(")");
            builder.AppendLine();
            builder.Append("VALUES");

            var lines = 0;
            foreach (var value in command.Values)
            {
                if (lines > 0)
                {
                    builder.Append(",");
                    builder.Append(Environment.NewLine);
                }
                first = true;
                foreach (var column in command.Columns)
                {
                    builder.Append(first ? "(" : ", ");
                    builder.Append(ReflectionHelper.GetPropertyValue(column.PropertyName, value));
                    first = false;
                }

                builder.Append(")");
                lines++;
            }

            return builder.ToString();
        }
    }
}
