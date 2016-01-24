using System.Collections.Generic;

namespace Syntax.Data.Dialects
{
    /// <summary>
    /// Defines the methods used to generate SQL from <see cref="SqlCommand{T}"/> objects.
    /// </summary>
    public interface ISqlDialect
    {
        /// <summary>
        /// Generates an SQL INSERT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        string Insert<T>(SqlInsert<T> command);

        /// <summary>
        /// Generates a SQL SELECT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        string Select<T>(SqlSelect<T> command);

        /// <summary>
        /// Generates a SQL UPDATE statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        string Update<T>(SqlUpdate<T> command);

        /// <summary>
        /// Generates a SQL DELETE statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        string Delete<T>(SqlDelete<T> command);

        /// <summary>
        /// Generates a bulk SQL INSERT statement.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TList">The type of the list.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        string BulkInsert<TItem, TList>(SqlBulkInsert<TItem, TList> command) where TList : IEnumerable<TItem>;
    }
}
