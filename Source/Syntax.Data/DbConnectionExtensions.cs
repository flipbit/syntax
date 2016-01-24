using System;
using System.Collections.Generic;
using System.Data;

namespace Syntax.Data
{
    public static class DbConnectionExtensions
    {
        public static SqlSelect<T> Select<T>(this IDbConnection connection)
        {
            return new SqlSelect<T>(connection, null);
        }

        public static SqlSelect<T> Select<T>(this IDbTransaction transaction)
        {
            return new SqlSelect<T>(transaction.Connection, transaction);
        }

        public static SqlInsert<T> Insert<T>(this IDbConnection connection, T target)
        {
            return new SqlInsert<T>(target, connection, null);
        }

        public static SqlInsert<T> Insert<T>(this IDbTransaction transaction, T target)
        {
            return new SqlInsert<T>(target, transaction.Connection, transaction);
        }

        public static SqlBulkInsert<T, IEnumerable<T>> BulkInsert<T>(this IDbConnection connection, IEnumerable<T> target)  where T : new()
        {
            return new SqlBulkInsert<T, IEnumerable<T>>(target, new T(), connection, null);
        }

        public static SqlBulkInsert<T, IEnumerable<T>> BulkInsert<T>(this IDbTransaction transaction, IEnumerable<T> target) where T : new()
        {
            return new SqlBulkInsert<T, IEnumerable<T>>(target, new T(), transaction.Connection, transaction);
        }

        public static SqlUpdate<T> Update<T>(this IDbConnection connection, T target)
        {
            return new SqlUpdate<T>(target, connection, null);
        }

        public static SqlUpdate<T> Update<T>(this IDbTransaction transaction, T target)
        {
            return new SqlUpdate<T>(target, transaction.Connection, transaction);
        }

        public static SqlDelete<T> Delete<T>(this IDbConnection connection)
        {
            return new SqlDelete<T>(connection, null);
        }

        public static SqlDelete<T> Delete<T>(this IDbTransaction transaction)
        {
            return new SqlDelete<T>(transaction.Connection, transaction);
        }

        /// <summary>
        /// Executes the specified SQL against the <see cref="IDbConnection" />.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public static IDbConnection ExecuteNonQuery(this IDbConnection connection, string sql, object parameters = null, IDbTransaction transaction = null, int timeout = 30)
        {
            var dictionary = parameters as IDictionary<string, object>;

            if (dictionary == null)
            {
                dictionary = parameters.ToDictionary();
            }

            return ExecuteNonQuery(connection, sql, dictionary, transaction, timeout);
        }

        private static IDbConnection ExecuteNonQuery(IDbConnection connection, string sql, IDictionary<string, object> parameters, IDbTransaction transaction, int timeout)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandTimeout = timeout;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    foreach (var key in parameters.Keys)
                    {
                        var parameter = command.CreateParameter();

                        parameter.ParameterName = key;
                        parameter.Value = parameters[key];

                        command.Parameters.Add(parameter);
                    }
                }

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    throw new DataException(exception, sql, command.Parameters);
                }
            }

            return connection;
        }


        /// <summary>
        /// Executes the specified SQL against the <see cref="IDbConnection" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(this IDbConnection connection, string sql, object parameters = null, IDbTransaction transaction = null, int timeout = 30)
        {
            T result;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandTimeout = timeout;
                command.Transaction = transaction;

                if (parameters != null)
                {
                    foreach (var parameter in SqlMapper.ObjectToParameters(command, parameters))
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                object dbResult;

                try
                {
                    dbResult = command.ExecuteScalar();
                }
                catch (Exception exception)
                {
                    throw new DataException(exception, sql, command.Parameters);
                }

                if (dbResult == null || dbResult == DBNull.Value)
                {
                    result = default (T);
                }
                else
                {
                    result = (T) Convert.ChangeType(dbResult, typeof (T));
                }
            }

            return result;
        }
    }
}
