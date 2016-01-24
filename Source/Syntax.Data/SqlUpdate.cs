using System.Data;

namespace Syntax.Data
{
    /// <summary>
    /// Builds an SQL UPDATE statement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlUpdate<T> : SqlCommand<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlInsert{T}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        public SqlUpdate(T target, IDbConnection connection, IDbTransaction transaction) : base(target, connection, transaction)
        {
        }

        /// <summary>
        /// Returns an SQL representation of this command.
        /// </summary>
        /// <returns></returns>
        public override string ToSql()
        {
            return Dialect.Update(this);
        }
    }
}
