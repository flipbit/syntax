using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Syntax.Data.Dialects;

namespace Syntax.Data
{
    /// <summary>
    /// Base SQL command object
    /// </summary>
    public abstract class SqlCommand<T>
    {
        /// <summary>
        /// Gets or sets the name of the primary table the SQL command
        /// runs against.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName { get; set; }

        /// <summary>
        /// Gets the parameters for the command.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public IList<DbParameter> Parameters { get; private set; }

        /// <summary>
        /// Gets the columns affected by the SQL command.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public IList<ColumnValue> Columns { get; private set; }

        /// <summary>
        /// Gets the SQL WHERE clauses for this command.
        /// </summary>
        /// <value>
        /// The clauses.
        /// </value>
        public IList<Clause> Clauses { get; private set; }

        /// <summary>
        /// Gets the SQL ORDER BY statements for this command.
        /// </summary>
        /// <value>
        /// The orders.
        /// </value>
        public IList<Order> Orders { get; private set; }

        /// <summary>
        /// Gets or sets the SQL dialect.
        /// </summary>
        /// <value>
        /// The SQL dialect.
        /// </value>
        public ISqlDialect Dialect { get; set; }

        /// <summary>
        /// The target object containing the values for this SQL Command
        /// </summary>
        protected readonly object Target;

        /// <summary>
        /// The connection
        /// </summary>
        protected readonly IDbConnection Connection;

        /// <summary>
        /// The transaction
        /// </summary>
        protected readonly IDbTransaction Transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommand{T}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        protected SqlCommand(object target, IDbConnection connection, IDbTransaction transaction)
        {
            Target = target;
            Connection = connection;
            Transaction = transaction;
            Orders = new List<Order>();
            Clauses = new List<Clause>();
            Columns = new List<ColumnValue>();
            Dialect = new AutomaticDialect(connection);
            Parameters = new List<DbParameter>();

            TableName = string.Format("[{0}]", typeof (T).Name);
        }

        /// <summary>
        /// Sets the table to extract the data from.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <returns></returns>
        public SqlCommand<T> From(string table)
        {
            TableName = table;

            return this;
        }

        /// <summary>
        /// Sets the table to extract the data from.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public SqlCommand<T> From(Type type)
        {
            return From(string.Format("[{0}]", type.Name));
        }

        /// <summary>
        /// Sets the table to extract the data from.
        /// </summary>
        /// <typeparam name="TTableType">The type of the table type.</typeparam>
        /// <returns></returns>
        public SqlCommand<T> From<TTableType>()
        {
            return From(typeof(TTableType));
        }

        private string GetParameterName(string prefix, int count)
        {
            return string.Concat("@", prefix, count);
        }

        /// <summary>
        /// Adds Count column to the Select.
        /// </summary>
        /// <returns></returns>
        public SqlCommand<T> Count()
        {
            var column = new ColumnValue
            {
                Name = "COUNT(1)",
                ParameterName = string.Empty
            };

            Columns.Add(column);

            return this;
        }

        public SqlCommand<T> Star()
        {
            var column = new ColumnValue
            {
                Name = "*"
            };

            Columns.Add(column);

            return this;
        }

        /// <summary>
        /// Adds a columns to the select.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public SqlCommand<T> Column(string columnName)
        {
            var column = new ColumnValue
            {
                Name = columnName,
                ParameterName = "@" + columnName
            };

            Columns.Add(column);

            return this;
        }

        public SqlCommand<T> Column(Expression<Func<T, object>> value)
        {
            var column = ColumnValue.Build(Target, value);

            Columns.Add(column);

            return this;
        }

        public SqlCommand<T> Column(Expression<Func<T, object>> value, string columnName)
        {
            var column = ColumnValue.Build(Target, value);

            column.Name = columnName;
            column.ParameterName = "@" + columnName;

            Columns.Add(column);

            return this;
        }

        public SqlCommand<T> Max(Expression<Func<T, object>> value)
        {
            var column = ColumnValue.Build(Target, value);

            column.Name = string.Format("MAX({0})", column.Name);

            Columns.Add(column);

            return this;
        }

        /// <summary>
        /// Adds a WHERE clause to this instance.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public SqlCommand<T> Where(Expression<Func<T, object>> expression)
        {
            return Where(expression, true);
        }

        public SqlCommand<T> Where(Expression<Func<T, object>> expression, bool onlyIf)
        {
            if (onlyIf)
            {
                var clause = Clause.Build(expression);

                clause.ParameterName = GetParameterName("w", Clauses.Count + 1);

                Clauses.Add(clause);
            }

            return this;
        }

        /// <summary>
        /// Adds a AND WHERE clause to this instance
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SqlCommand<T> AndWhere(Expression<Func<T, object>> expression)
        {
            var clause = Clause.Build(expression);
            
            clause.ParameterName = GetParameterName("w", Clauses.Count + 1);

            Clauses.Add(clause);

            return this;
        }

        public SqlCommand<T> OrderBy(Expression<Func<T, object>> expression)
        {
            var order = Order.Build(expression);

            Orders.Add(order);

            return this;
        }

        /// <summary>
        /// Returns an SQL representation of this command.
        /// </summary>
        /// <returns></returns>
        public abstract string ToSql();

        /// <summary>
        /// Gets a list of all the parameters in this command.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> ToParameters()
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var column in Columns)
            {
                if (!string.IsNullOrEmpty(column.ParameterName))
                {
                    dictionary.Add(column.ParameterName, column.Value);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Syntax.Data.DataException">null</exception>
        public IEnumerable<T> Query()
        {
            try
            {
                return Connection.Query<T>(ToSql());
            }
            catch (Exception exception)
            {
                throw new DataException(exception, ToSql(), null);
            }
        }

        public IEnumerable<TQuery> Query<TQuery>()
        {
            try
            {
                return Connection.Query<TQuery>(ToSql());
            }
            catch (Exception exception)
            {
                throw new DataException(exception, ToSql(), null);
            }
        }

        public void ExecuteNonQuery()
        {
            Connection.ExecuteNonQuery(ToSql(), ToParameters(), Transaction);
        }

        public TIdentity ExecuteScalar<TIdentity>()
        {
            return Connection.ExecuteScalar<TIdentity>(ToSql(), ToParameters(), Transaction);
        }
    }
}
