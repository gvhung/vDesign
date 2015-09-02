using Base.Service;
using Base.Service.Log;
using System;
using System.IO;
using System.Text;

namespace WebUI.Concrete
{
    public class LogService : ILogService
    {
        private readonly IPathHelper _pathHelper;

        public LogService(IPathHelper pathHelper)
        {
            _pathHelper = pathHelper;

            string logDir = pathHelper.GetLogDirectory();

            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
        }

        private string GetFileName()
        {
            return Path.Combine(_pathHelper.GetLogDirectory(), String.Format("log-{0:dd-MM-yyyy}.log", DateTime.Now));
        }

        public void Log(Exception exception)
        {
            this.Log(this.LogException(exception));
        }

        public void Log(Exception exception, string message)
        {
            this.Log(String.Format("{0} ({1})", this.LogException(exception), message));
        }

        private string LogException(Exception exception)
        {
            StringBuilder sb = new StringBuilder();

            this.LogExceptionImpl(exception, sb);

            return sb.ToString();
        }

        private void LogExceptionImpl(Exception exception, StringBuilder sb)
        {
            sb.AppendFormat("{0}", exception.Message);

            if (exception.InnerException != null)
            {
                sb.Append(" -> ");

                this.LogExceptionImpl(exception.InnerException, sb);
            }
        }

        public void Log(string message)
        {
            string msg = String.Format("{0:HH:mm:ss} -- {1}{2}", DateTime.Now, message, Environment.NewLine);

            string _fileName = GetFileName();

            try
            {
                if (!File.Exists(_fileName))
                {
                    File.WriteAllText(_fileName, msg);
                }
                else
                {
                    File.AppendAllText(_fileName, msg);
                }
            }
            catch (Exception)
            {
                //TODO : плохо тут, надо бы обработать ошибку
            }
        }
    }
}