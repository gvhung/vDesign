using System;
using System.Text;

namespace Framework
{
    public static class ExceptionExtensions
    {
        private static void LogExceptionImpl(Exception exception, StringBuilder sb)
        {
            if (exception != null)
            {
                sb.AppendFormat("{0}", exception.Message);

                if (exception.InnerException != null)
                {
                    sb.Append(" -> ");

                    LogExceptionImpl(exception.InnerException, sb);
                }    
            }
        }

        public static string ToStringWithInner(this Exception exception)
        {
            StringBuilder sb = new StringBuilder();

            LogExceptionImpl(exception, sb);

            return sb.ToString();
        }

        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }
    }
}