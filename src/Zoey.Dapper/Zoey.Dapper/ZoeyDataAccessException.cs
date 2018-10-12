using Dapper;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Zoey.Dapper
{
    [Serializable]
    internal class ZoeyDataAccessException : Exception
    {
        public ZoeyDataAccessException(Exception innerException, string connectionString, string sqlText, object parameters)
            : base(BuildErrorMsg(innerException.Message, connectionString, sqlText, parameters), innerException)
        {
        }

        private static string BuildErrorMsg(string errorMsg, string connectionString, string sqlText, object parameters)
        {
            var msg = new StringBuilder();
            msg.AppendLine(errorMsg);
            msg.AppendLine($"[连接字符串]:{connectionString}");
            msg.AppendLine($"[SQL语句]:{sqlText}");

            if (parameters != null)
            {
                msg.AppendLine("[参数]:");

                if (parameters is DynamicParameters)
                    msg.AppendLine(parameters.ToString());
                else
                {
                    try
                    {
                        var sb = new StringBuilder();
                        var ser = new XmlSerializer(parameters.GetType());
                        using (var writer = new StringWriter(sb))
                        {
                            ser.Serialize(writer, parameters);
                            msg.AppendLine(writer.ToString());
                        }
                    }
                    catch
                    {
                        msg.AppendLine(parameters.ToString());
                    }
                }
            }
            return msg.ToString();
        }
    }
}