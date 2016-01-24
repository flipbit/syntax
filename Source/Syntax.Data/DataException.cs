using System;
using System.Collections;
using System.Data.Common;
using System.Text;

namespace Syntax.Data
{
    public class DataException : Exception
    {
        private readonly string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataException" /> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters"></param>
        public DataException(Exception innerException, string sql, ICollection parameters) : base(string.Empty, innerException)
        {
            var builder = new StringBuilder();

            builder.AppendLine("SQL Data Exception:");
            builder.AppendLine(innerException.Message);
            builder.AppendLine();
            builder.AppendLine("Running SQL:");
            builder.AppendLine(sql);
            builder.AppendLine();

            if (parameters != null && parameters.Count > 0)
            {
                builder.AppendLine("Parameters:");
                builder.AppendLine();
                foreach (DbParameter parameter in parameters)
                {
                    builder.AppendLine(parameter.ParameterName + " = '" + parameter.Value + "'");
                }
            }

            message = builder.ToString();
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message
        {
            get { return message; }
        }
    }
}
