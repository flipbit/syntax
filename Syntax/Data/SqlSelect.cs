using System.Data;
using System.Text;

namespace Syntax.Data
{
    /// <summary>
    /// Helper class to build SQL Selects programmatically.
    /// </summary>
    /// <remarks>
    /// When link funky selectors are used, column and table names are wrapped with [] (e.g. Column(x => x.Key) will 
    /// render as [Key]).   Potential SQL injection risk! Always use @parameters when dealing with string predicates!
    /// </remarks>
    public class SqlSelect<T> : SqlCommand<T>
    {        
        /// <summary>
        /// Gets or sets a value indicating whether to lock the table during a SELECT operation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [with no lock]; otherwise, <c>false</c>.
        /// </value>
        public bool WithNoLock { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelect{T}"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        public SqlSelect(IDbConnection connection, IDbTransaction transaction) : base(null, connection, transaction)
        {
        }

        /// <summary>
        /// Specifies the WITH (NOLOCK) table option.
        /// </summary>
        /// <returns></returns>
        public SqlSelect<T> UseWithNoLock()
        {
            WithNoLock = true;

            return this;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToSql()
        {
            return Dialect.Select(this);
        }
    }
}
