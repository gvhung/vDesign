using System;

namespace Base.Service.Log
{
    public interface ILogService
    {
        void Log(Exception exception);
        void Log(Exception exception, string message);
        void Log(string message);
    }
}