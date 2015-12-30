using System.Data;

namespace Syntax.Data
{
    /// <summary>
    /// Builds a SQL UPDATE statement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlDelete<T> : SqlCommand<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInsert{T}" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        public SqlDelete(IDbConnection connection, IDbTransaction transaction) : base(null, connection, transaction)
        {
        }

        /// <summary>
        /// Returns an SQL representation of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToSql()
        {
            return Dialect.Delete(this);
        }
    }
}
