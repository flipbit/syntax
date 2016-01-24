using System;
using System.Collections.Generic;
using System.Reflection;

namespace Syntax.Data
{
    /// <summary>
    /// Extension methods for objects
    /// </summary>
    public static class ObjectExtensions
    {
        public static string ToSqlQuotedString(this object value)
        {
            string result;

            if (value == null)
            {
                result = "NULL";
            }
            else if (value is string)
            {
                result = string.Format("'{0}'", value);
            }
            else if (value is bool)
            {
                if ((bool)value)
                {
                    result = "1";
                }
                else
                {
                    result = "0";
                }
            }
            else if (value is DateTime)
            {
                result = string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);
            }
            else
            {
                result = value.ToString();
            }

            return result;
        }

        public static IDictionary<string, object> ToDictionary(this object @object)
        {
            var values = new Dictionary<string, object>();

            if (@object != null)
            {
                var properties = @object.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var property in properties)
                {
                    values.Add(property.Name, property.GetValue(@object, null));
                }
            }

            return values;
        }
    }
}
