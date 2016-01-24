namespace Syntax.Data.Dialects
{
    /// <summary>
    /// SQL Dialect for Microsoft SQL
    /// </summary>
    public class MsSqlDialect : SqlDialect
    {
        /// <summary>
        /// Generates 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public override string Insert<T>(SqlInsert<T> command)
        {
            var sql = base.Insert(command);

            return string.Concat(sql, "SELECT SCOPE_IDENTITY();");
        }
    }
}
