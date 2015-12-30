using System.Data;
using Syntax.Data.Dialects;

namespace Syntax.Data
{
    /// <summary>
    /// Builds a SQL INSERT statement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlInsert<T> : SqlCommand<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInsert{T}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        public SqlInsert(T target, IDbConnection connection, IDbTransaction transaction) : base(target, connection, transaction)
        {
        }

        public override string ToSql()
        {
            return Dialect.Insert(this);
        }
    }
}
