using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Syntax.Data
{
    public static class SqlMapper
    {
        public static IQueryable<T> Query<T>(this IDbConnection connection, string sql)
        {
            var results = new List<T>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        T result;

                        if (typeof(T).IsValueType || typeof(T) == typeof(string))
                        {
                            result = (T)Convert.ChangeType(reader[0], typeof(T));
                        }
                        else
                        {
                            result = MapReaderToObject<T>(reader);
                        }

                        results.Add(result);
                    }
                }
            }

            return results.AsQueryable();
        }

        public static T MapReaderToObject<T>(IDataReader reader)
        {
            var result = Activator.CreateInstance<T>();

            foreach (var property in typeof(T).GetProperties())
            {
                if (!HasColumn(reader, property.Name)) continue;

                if (!object.Equals(reader[property.Name], DBNull.Value))
                {
                    object value;

                    if (property.PropertyType.IsEnum)
                    {
                        value = Enum.Parse(property.PropertyType, reader[property.Name].ToString());
                    }
                    else if (property.PropertyType.IsGenericType &&
                             property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var actual = Nullable.GetUnderlyingType(property.PropertyType);

                        value = Convert.ChangeType(reader[property.Name], actual);
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        var ticks = Convert.ToInt64(reader[property.Name]);

                        value = new DateTime(ticks);
                    }
                    else
                    {
                        value = Convert.ChangeType(reader[property.Name], property.PropertyType);
                    }

                    property.SetValue(result, value, null);
                }
            }

            return result;
        }

        public static IEnumerable<IDbDataParameter> ObjectToParameters(IDbCommand command, object @object)
        {
            var values = new List<IDbDataParameter>();

            if (@object != null)
            {
                var properties = @object.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var property in properties)
                {
                    var parameter = command.CreateParameter();

                    parameter.ParameterName = property.Name;
                    parameter.Value = property.GetValue(@object, null);

                    values.Add(parameter);
                }
            }

            return values;
        }

        public static bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}