using System.Collections.Generic;
using System.Data;

namespace Syntax.Data
{
    /// <summary>
    /// Builds a SQL BULK INSERT statement
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <typeparam name="TList">The type of the list.</typeparam>
    public class SqlBulkInsert<TItem, TList> : SqlCommand<TItem> where TList : IEnumerable<TItem>
    {
        /// <summary>
        /// Gets the values to insert.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IEnumerable<TItem> Values { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInsert{TItem, TList}" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="type">The type.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        public SqlBulkInsert(TList values, TItem type, IDbConnection connection, IDbTransaction transaction) : base(type, connection, transaction)
        {
            Values = values;
        }

        /// <summary>
        /// Returns an SQL representation of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToSql()
        {
            return Dialect.BulkInsert(this);
        }
    }
}