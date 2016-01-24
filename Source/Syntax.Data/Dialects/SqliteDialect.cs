namespace Syntax.Data.Dialects
{
    /// <summary>
    /// SQLite SQL Dialect
    /// </summary>
    public class SqliteDialect : SqlDialect
    {
        /// <summary>
        /// Generates an SQL INSERT statement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public override string Insert<T>(SqlInsert<T> command)
        {
            var sql = base.Insert(command);

            return string.Concat(sql, "SELECT last_insert_rowid();");
        }
    }
}
