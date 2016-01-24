using System;
using System.Collections.Generic;
using System.Data;

namespace Syntax.Data.Dialects
{
    /// <summary>
    /// Dialect that generates SQL based upon the IDbConnection type
    /// </summary>
    public class AutomaticDialect : ISqlDialect
    {
        /// <summary>
        /// Gets the SQL dialect.
        /// </summary>
        /// <value>
        /// The SQL dialect.
        /// </value>
        public ISqlDialect SqlDialect { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticDialect"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public AutomaticDialect(IDbConnection connection)
        {
            var typeName = connection.GetType().Name;

            switch (typeName)
            {
                case "SQLiteConnection":
                    SqlDialect = new SqliteDialect();
                    break;

                case "SqlConnection":
                    SqlDialect = new MsSqlDialect();
                    break;

                default:
                    throw new ArgumentException("Can't create a ISqlDialect for type: " + typeName);
            }
        }

        /// <summary>
        /// Generates an SQL INSERT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public string Insert<T>(SqlInsert<T> command)
        {
            return SqlDialect.Insert(command);
        }

        /// <summary>
        /// Generates a SQL SELECT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public string Select<T>(SqlSelect<T> command)
        {
            return SqlDialect.Select(command);
        }

        /// <summary>
        /// Generates a SQL UPDATE statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public string Update<T>(SqlUpdate<T> command)
        {
            return SqlDialect.Update(command);
        }

        /// <summary>
        /// Generates a SQL DELETE statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public string Delete<T>(SqlDelete<T> command)
        {
            return SqlDialect.Delete(command);
        }

        /// <summary>
        /// Generates a bulk SQL INSERT statement.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TList">The type of the list.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public string BulkInsert<TItem, TList>(SqlBulkInsert<TItem, TList> command) where TList : IEnumerable<TItem>
        {
            return SqlDialect.BulkInsert(command);
        }
    }
}
